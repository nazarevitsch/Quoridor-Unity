using AStar.models.AI.PathFinder;

namespace AStar.models.AI.MoveStrategies.MoveStrategies
{
    public interface IMoveStrategy<TCoord> where TCoord : Coords
    {
        public void MakeMove(TCoord curPlayer, TCoord enemyPlayer, TCoord [][] points);
    }
}