using AStar.models.AI.PathFinder;

namespace AStar.models.AI.MoveStrategies.WallStrategies
{
    public class WallScriptStrategy<TCoord> : IWallPlacementStrategy<TCoord> where TCoord : Coords
    {
        // Works only for white side
        public Wall GetWallToPlace<TWallCreateStrategy>(TCoord curPlayer, TCoord enemyPlayer, TCoord[][] points) where TWallCreateStrategy : IWallStrategy, new()
        {
            throw new System.NotImplementedException();
        }
    }
}