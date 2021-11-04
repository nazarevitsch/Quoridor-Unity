using AStar.models.AI.PathFinder;

namespace AStar.models.AI.MoveStrategies.MoveStrategies
{
    public class WhiteSideStrategy<TCoords> : IMoveStrategy<TCoords> where TCoords : Coords
    {
        public void MakeMove(TCoords curPlayer, TCoords enemyPlayer, TCoords[][] points)
        {
            throw new System.NotImplementedException();
        }
    }
}