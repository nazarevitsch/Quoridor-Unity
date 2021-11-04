using System;
using System.Collections.Generic;

namespace AStar.models
{
    public enum WallType
    {
        Vertical = 1,
        Horizontal = 2
    }
    public class Wall : IEquatable<Wall>
    {
        private Dictionary<char, int> FromCharToIntMapX { get; } = new()
        {
            {'s', 1},
            {'t', 3},
            {'u', 5},
            {'v', 7},
            {'w', 9},
            {'x', 11},
            {'y', 13},
            {'z', 15},
        };
        private Dictionary<int, char> FromIntToCharMapX { get; } = new()
        {
            {1, 's'},
            {3, 't'},
            {5, 'u'},
            {7, 'v'},
            {9, 'w'},
            {11, 'x'},
            {13, 'y'},
            {15, 'z'},
        };
        
        private Dictionary<char, int> FromCharToIntMapY { get; } = new()
        {
            {'1', 1},
            {'2', 3},
            {'3', 5},
            {'4', 7},
            {'5', 9},
            {'6', 11},
            {'7', 13},
            {'8', 15}
        };
        
        private Dictionary<int, char> FromIntToCharMapY { get; } = new()
        {
            {1, '1'},
            {3, '2'},
            {5, '3'},
            {7, '4'},
            {9, '5'},
            {11, '6'},
            {13, '7'},
            {15, '8'}
        };

        private string WallTypeAsString => WallType == WallType.Horizontal ? "h" : "v";
        public string AsString => $"{FromIntToCharMapX[Point.X]}{FromIntToCharMapY[Point.Y]}{WallTypeAsString}";
        public Point Point { get; }
        public WallType WallType { get; }
        public Wall(Point point, WallType wallType)
        {
            Point = point;
            WallType = wallType;
        }
        

        public Wall(string wallString)
        {
            wallString = wallString.ToLower();
            Point = new Point
            {
                X = FromCharToIntMapX[wallString[0]],
                Y = FromCharToIntMapY[wallString[1]]
            };
            WallType = wallString[2] == 'v' ? WallType.Vertical : WallType.Horizontal;
        }

        public bool Equals(Wall other)
        {
            if (other is null) return false;
            return AsString == other.AsString || (Point.X == other.Point.X && Point.Y == other.Point.Y);
        }
    }
}