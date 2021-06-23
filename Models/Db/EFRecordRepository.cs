using minesweeper.Models.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace minesweeper.Models.Db
{
    public class EFRecordRepository : IRecordRepository
    {
        private AppDbContext context;
        public EFRecordRepository(AppDbContext context) => this.context = context;
        public void AddRecord(Record record)
        {
            context.Add(record);
            context.SaveChanges();
        }

        public Statistics GetStatistics(string userId, FieldTypes type)
        {
            var privateRecords = context.Records.Where(r => r.UserId == userId && r.Type == type);
            
            var wins = privateRecords.Where(r => r.Success);
            var durations = wins.Select(r => r.Duration).ToList();

            TimeSpan averageTime = new TimeSpan();
            TimeSpan bestTime = new TimeSpan();
            if (durations.Count() != 0)
            {
                averageTime = TimeSpan.FromSeconds(durations.Select(duration => duration.TotalSeconds).Average());
                bestTime = durations.Min();
            }

            var publicRecords = context.Records
                .Include(record => record.User)
                .Where(record => record.Success && record.Type == type)
                .AsEnumerable()
                .GroupBy(record => record.User)
                .Select(groupedRecords => new { user = groupedRecords.Key, duration = groupedRecords.Min(r => r.Duration) })
                .OrderBy(usersRecords => usersRecords.duration);

            var ownPlace = publicRecords.Select((record, index) => new { record, index })
                                        .Where(x => x.record.user.Id == userId)
                                        .Select(x => x.index + 1)
                                        .FirstOrDefault();
            return new Statistics
            {
                Attempts = privateRecords.Count(),
                Wins = wins.Count(),
                AverageTime = averageTime,
                BestTime = bestTime,
                Place = ownPlace,
                PrivateRecords = privateRecords.OrderByDescending(r => r.Date).Select(r => new PrivateStatRecord(r)),
                PublicRecords = publicRecords.Select(r => new PublicStatRecord(r.user, r.duration))
            };
        }
    }
}
