using System;
using AStar.models.AI.PathFinder;

namespace AStar.models.AI.MoveStrategies.WallStrategies
{
    public class WallStreetStrategy<TCoord> : IWallPlacementStrategy<TCoord> where TCoord : Coords
    {
        public Wall GetWallToPlace<TWallCreateStrategy>(TCoord curPlayer, TCoord enemyPlayer, TCoord[][] points) where TWallCreateStrategy : IWallStrategy, new()
        {
            var wallStrategy = new TWallCreateStrategy();
            var random = new Random();
            if (random.Next(1, 10) < 8) return null;
            var wall = wallStrategy.GetWall(curPlayer, enemyPlayer, points);
            return IsWallAlreadyPlaced(wall, points) ? null : wall;
        }

        private bool IsWallAlreadyPlaced(Wall wall, TCoord [][] coords)
        {
            if (wall.WallType == WallType.Vertical)
            {
                var leftWall = (string) coords[wall.Point.Y][wall.Point.X - 1].GetInfo();
                var rightWall = (string) coords[wall.Point.Y][wall.Point.X + 1].GetInfo();
                if (leftWall == "Blocked" || leftWall == "HalfBlocked")
                {
                    return true;
                }
                return rightWall == "Blocked" || rightWall == "HalfBlocked";
            }
            var upperWall = (string) coords[wall.Point.Y - 1][wall.Point.X].GetInfo();
            var lowerWall = (string) coords[wall.Point.Y + 1][wall.Point.X].GetInfo();
            if (upperWall == "Blocked" || upperWall == "HalfBlocked")
            {
                return true;
            }
            return lowerWall == "Blocked" || lowerWall == "HalfBlocked";
        }
    }
}