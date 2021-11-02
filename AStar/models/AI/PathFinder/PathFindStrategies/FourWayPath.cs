using System;
using System.Collections.Generic;
using System.Linq;

namespace AStar.models.AI.PathFinder.PathFindStrategies
{
    public class FourWayPath : IPathFinder
    {
        public List<TCoords> Find<TCoords>(TCoords current, TCoords[][] coords) where TCoords : Coords, new()
        {
            var sizeY = coords.Length;
            if (sizeY == 0) throw new Exception("Field is empty");
            var sizeX = coords[0].Length;
            return new List<TCoords>
                {
                    current.Y - 1 < 0 ? null : new TCoords {X = current.X, Y = current.Y - 1}, // TOP
                    current.X - 1 < 0 ? null : new TCoords {X = current.X - 1, Y = current.Y}, // MIDDLE-LEFT
                    current.X + 1 >= sizeX ? null : new TCoords {X = current.X + 1, Y = current.Y}, // MIDDLE-RIGHT
                    current.Y + 1 >= sizeY ? null : new TCoords {X = current.X, Y = current.Y + 1}, // BOTTOM-MIDDLE
                }
                .Where(coord => coord is not null && (string) coords[coord.Y][coord.X]?.GetInfo() != "1")
                .ToList();
        }
    }
}