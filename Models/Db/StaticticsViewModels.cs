using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using minesweeper.Models.Game;

namespace minesweeper.Models.Db
{
    public class Statistics
    {
        public int Attempts { get; set; }
        public int Wins { get; set; }
        public int Losses => Attempts - Wins;
        public TimeSpan AverageTime { get; set; }
        public TimeSpan BestTime { get; set; }
        public int Place { get; set; }
        public IEnumerable<PrivateStatRecord> PrivateRecords { get; set; }
        public IEnumerable<PublicStatRecord> PublicRecords { get; set; }
    }
    public class PrivateStatRecord
    {
        public PrivateStatRecord() { }
        public PrivateStatRecord(Record record)
        {
            Date = record.Date.ToString();
            Duration = $"{Math.Floor(record.Duration.TotalMinutes):00}:{record.Duration.Seconds:00}";
            Success = record.Success ? "Победа" : "Поражение";
        }
        public string Date { get; set; }
        public string Duration { get; set; }
        public string Success { get; set; }
    }
    public class PublicStatRecord
    {
        public PublicStatRecord() { }
        public PublicStatRecord(AppUser user, TimeSpan duration)
        {
            User = user.UserName;
            Duration = $"{Math.Floor(duration.TotalMinutes):00}:{duration.Seconds:00}";
        }
        public string User { get; set; }
        public string Duration { get; set; }
    }

}
