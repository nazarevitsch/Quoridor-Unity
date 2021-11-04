using AStar.models.AI.PathFinder;

namespace AStar.models.AI.MoveStrategies.WallStrategies
{
    public interface IWallPlacementStrategy<TCoord> where TCoord: Coords
    {
        public Wall GetWallToPlace<TWallCreateStrategy>(TCoord curPlayer, TCoord enemyPlayer, TCoord[][] points) where TWallCreateStrategy : IWallStrategy, new();
    }
}