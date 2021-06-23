using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using minesweeper.Models.Game;

namespace minesweeper.Models.Db
{
    public interface IRecordRepository
    {
        void AddRecord(Record record);
        Statistics GetStatistics(string userId, FieldTypes type);
    }
}
