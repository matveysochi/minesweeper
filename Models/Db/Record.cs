using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using minesweeper.Models.Game;


namespace minesweeper.Models.Db
{
    public class Record
    {
        public Record() { }
        public Record(GameModel model, AppUser user)
        {
            User = user;
            Date = model.StartTime;
            Duration = model.EndTime - model.StartTime;
            Success = model.Win;
            Type = model.Type.Type;
        }
        public long Id { get; set; }
        public AppUser User { get; set; }
        public string UserId { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan Duration { get; set; }
        public FieldTypes Type { get; set; }
        public bool Success { get; set; }
    }
}
