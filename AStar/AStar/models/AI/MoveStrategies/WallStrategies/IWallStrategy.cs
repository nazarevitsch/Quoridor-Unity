using AStar.models.AI.PathFinder;

namespace AStar.models.AI.MoveStrategies.WallStrategies
{
    public interface IWallStrategy
    {
        public Wall GetWall<TCoord>(TCoord curPlayer, TCoord enemyPlayer, TCoord[][] points) where TCoord : Coords;
    }
}