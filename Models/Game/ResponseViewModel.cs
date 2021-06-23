using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace minesweeper.Models.Game
{
    public class Response
    {
        public Response(GameModel model)
        {
            Cells = model.ClientCells;
            Type = model.Type;
            GameStart = model.GameStart;
            GameEnd = model.GameEnd;
            Win = model.Win;
            Time = !GameStart ? 0 : (int) (GameEnd ? (model.EndTime - model.StartTime) : (DateTime.Now - model.StartTime)).TotalSeconds;
        }
        public Cell[][] Cells { get; set; }
        public FieldType Type { get; set; }
        public int Time { get; set; }
        public bool GameStart { get; set; } = false;
        public bool GameEnd { get; set; } = false;
        public bool Win { get; set; } = false;
    }
}
