using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace minesweeper.Models.Game
{
    public class GameModel
    {
        public Cell[][] ServerCells { get; set; }
        public Cell[][] ClientCells { get; set; }
        private Cell this[int x, int y]
        {
            get { return ServerCells[y][x]; }
        }
        private IEnumerable<Cell> cellsArea(int xEnd, int yEnd, int xStart = 0, int yStart = 0)
        {
            for (int y = yStart; y <= yEnd; y++)
            {
                for (int x = xStart; x <= xEnd; x++)
                {
                    if (x < 0 || x >= Width || y < 0 || y >= Height) continue;
                    yield return this[x, y];
                }
            }
        }
        private List<Cell> neighbСells(int x, int y)
        {
            return cellsArea(x + 1, y + 1, x - 1, y - 1).Where(cell => !(cell.x == x && cell.y == y)).ToList();
        }
        private List<Cell> allСells()
        {
            return cellsArea(Width, Height).ToList();
        }
        private int Width => Type.Width;
        private int Height => Type.Height;
        private int BombCount => Type.BombCount;


        public FieldType Type { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public bool GameStart { get; set; } = false;
        public bool GameEnd { get; set; } = false;
        public bool Win { get; set; } = false;
        public bool Registred { get; set; } = false;

        public GameModel() { }
        public GameModel(FieldTypes type)
        {
            Type = FieldType.StandartFields[type];
            
            ServerCells = new Cell[Height][];
            ClientCells = new Cell[Height][];

            for (int y = 0; y < Height; y++)
            {
                ServerCells[y] = new Cell[Width];
                ClientCells[y] = new Cell[Width];
                
                for (int x = 0; x < Width; x++)
                {
                    ServerCells[y][x] = new Cell(x, y);
                    ClientCells[y][x] = new Cell(x, y);
                }
            }
        }

        public Response Response() => new Response(this);
        public void Start(int x, int y)
        {
            SetField(x, y);
            StartTime = DateTime.Now;
            GameStart = true;
        }
        public void End(bool win)
        {
            GameEnd = true;
            EndTime = DateTime.Now;
            Win = win;
        }

        public void Operate(int x, int y, int action)
        {
            if (GameEnd || x < 0 || x >= Width || y < 0 || y >= Height || !(new List<int> { 0, 1, 2 }.Contains(action))) return;

            if (!GameStart) Start(x, y);
            if (action == 0) OpenCell(x, y);
            if (action == 2) SetFlag(x, y);
            if (action == 1) OpenAroundDig(x, y);
        }
        public void SetField(int cx, int cy)
        {
            var rand = new Random();
            int bombSet = 0;
            while (bombSet < BombCount)
            {
                int x = rand.Next(0, Width - 1);
                int y = rand.Next(0, Height - 1);
                if (!this[x, y].IsBomb && !(cx == x && cy == y))
                {
                    this[x, y].IsBomb = true;
                    bombSet++;
                }
            }
            allСells().ForEach(cell => cell.BombAround = neighbСells(cell.x, cell.y).Where(cell => cell.IsBomb).Count());
        }

        public void SetFlag(int x, int y)
        {
            Cell cell = this[x, y];
            if (cell.IsOpen) return;
            cell.IsFlag = !cell.IsFlag;
            
            ClientCells[y][x].IsFlag = cell.IsFlag;
        }

        public void OpenAroundDig(int x, int y)
        {
            Cell cell = this[x, y];
            if (!cell.IsOpen || cell.BombAround == 0) return;

            var neneighbors = neighbСells(x, y);
            if (neneighbors.Where(cell => cell.IsFlag).Count() == cell.BombAround)
            {
                neneighbors.ForEach(cell => OpenCell(cell.x, cell.y));
            }
        }

        public void OpenCell(int x, int y)
        {
            Cell cell = this[x, y];
            if (cell.IsOpen || cell.IsFlag) return;
            ClientCells[y][x] = cell;

            cell.IsOpen = true;

            if (cell.IsBomb)
            {
                End(win: false);
                return;
            }

            if (cell.BombAround == 0) neighbСells(x, y).ForEach(cell => OpenCell(cell.x, cell.y));

            var notOpen = allСells().Where(cell => !cell.IsOpen).ToList();
            if (notOpen.Count() == BombCount)
            {
                notOpen.ForEach(cell => {
                    cell.IsFlag = true;
                    ClientCells[cell.y][cell.x] = cell;
                });
                End(win: true);
            }
        }
    }
}
