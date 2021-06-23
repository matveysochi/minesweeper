using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace minesweeper.Models.Game
{
    public class FieldType
    {
        public static Dictionary<FieldTypes, FieldType> StandartFields => new Dictionary<FieldTypes, FieldType>
        {
            [FieldTypes.Новичек] = new FieldType(FieldTypes.Новичек, 8, 8, 10),
            [FieldTypes.Средний] = new FieldType(FieldTypes.Средний, 20, 20, 40),
            [FieldTypes.Сложный] = new FieldType(FieldTypes.Сложный, 40, 20, 99)
        };
        public FieldType(FieldTypes type, int width, int height, int bombCount)
        {
            Width = width;
            Height = height;
            BombCount = bombCount;
            Type = type;
        }
        public FieldTypes Type { get; }
        public int Width { get; }
        public int Height { get; }
        public int BombCount { get; }
    }
    public enum FieldTypes
    {
        Новичек,
        Средний,
        Сложный
    }
}
