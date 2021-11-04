using AStar.models.AI.PathFinder;

namespace AStar.models.AI.MoveStrategies.WallStrategies
{
    public class UnderPlayerWallStrategy : IWallStrategy
    {
        public Wall GetWall<TCoord>(TCoord curPlayer, TCoord enemyPlayer, TCoord[][] points) where TCoord : Coords
        {
            return new(new Point {X = enemyPlayer.X - 3, Y = enemyPlayer.Y + 1}, WallType.Horizontal);
        }
    }
}