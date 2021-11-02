using System;
using System.Collections.Generic;

namespace AStar.models
{
    public class Move
    {
        private Dictionary<char, int> FromCharToIntMapX { get; } = new()
        {
            {'a', 0},
            {'b', 2},
            {'c', 4},
            {'d', 6},
            {'e', 8},
            {'f', 10},
            {'g', 12},
            {'h', 14},
            {'i', 16},
        };
        private Dictionary<int, char> FromIntToCharMapX { get; } = new()
        {
            {0, 'a'},
            {2, 'b'},
            {4, 'c'},
            {6, 'd'},
            {8, 'e'},
            {10, 'f'},
            {12, 'g'},
            {14, 'h'},
            {16, 'i'}
        };
        
        private Dictionary<char, int> FromCharToIntMapY { get; } = new()
        {
            {'1', 0},
            {'2', 2},
            {'3', 4},
            {'4', 6},
            {'5', 8},
            {'6', 10},
            {'7', 12},
            {'8', 14},
            {'9', 16},
        };
        
        private Dictionary<int, char> FromIntToCharMapY { get; } = new()
        {
            {0, '1'},
            {2, '2'},
            {4, '3'},
            {6, '4'},
            {8, '5'},
            {10, '6'},
            {12, '7'},
            {14, '8'},
            {16, '9'}
        };
        
        public Point Point { get; }

        public string AsString => $"{FromIntToCharMapX[Point.X]}{FromIntToCharMapY[Point.Y]}";

        public Move(Point point)
        {
            Point = point;
        }

        public Move(string stringMove)
        {
            Point = new Point
            {
                X = FromCharToIntMapX[stringMove[0]],
                Y = FromCharToIntMapY[stringMove[1]]
            };
        }
        
        
        
    }
}