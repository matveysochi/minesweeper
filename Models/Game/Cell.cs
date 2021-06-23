namespace minesweeper.Models.Game
{
    public class Cell
    {
        public Cell(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        public int x { get; set; }
        public int y { get; set; }
        public bool IsBomb { get; set; } = false;
        public bool IsFlag { get; set; } = false;
        public bool IsOpen { get; set; } = false;
        public int BombAround { get; set; } = 0;
    }
}
