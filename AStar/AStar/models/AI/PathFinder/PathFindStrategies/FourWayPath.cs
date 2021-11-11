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
            var stepLength = 2;
            return new List<TCoords>
                {
                    current.Y - stepLength < 0 ? null : new TCoords {X = current.X, Y = current.Y - stepLength}, // TOP
                    current.X - stepLength < 0 ? null : new TCoords {X = current.X - stepLength, Y = current.Y}, // MIDDLE-LEFT
                    current.X + stepLength > sizeX ? null : new TCoords {X = current.X + stepLength, Y = current.Y}, // MIDDLE-RIGHT
                    current.Y + stepLength > sizeY ? null : new TCoords {X = current.X, Y = current.Y + stepLength}, // BOTTOM-MIDDLE
                }
                .Where((coord, index) =>
                {
                    if (coord is null) return false;
                    var tag = index switch
                    {
                        0 => (string) coords[coord.Y + 1][coord.X]?.GetInfo(),
                        1 => (string) coords[coord.Y][coord.X + 1]?.GetInfo(),
                        2 => (string) coords[coord.Y][coord.X - 1]?.GetInfo(),
                        3 => (string) coords[coord.Y - 1][coord.X]?.GetInfo(),
                        _ => ""
                    };
                    return tag != "Blocked" && tag != "HalfBlocked";
                })
                .ToList();
        }
    }
}