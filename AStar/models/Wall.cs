using System.Collections.Generic;

namespace AStar.models
{
    public enum WallType
    {
        Vertical = 1,
        Horizontal = 2
    }
    public class Wall
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
        
        public Point Point { get; }

        public WallType WallType { get; }
        
        public Wall(Point point)
        {
            Point = point;
        }
        

        public Wall(string wallString)
        {
            Point = new Point
            {
                X = FromCharToIntMapX[wallString[0]],
                Y = FromCharToIntMapY[wallString[1]]
            };
            WallType = wallString[2] == 'v' ? WallType.Vertical : WallType.Horizontal;
        }
    }
}