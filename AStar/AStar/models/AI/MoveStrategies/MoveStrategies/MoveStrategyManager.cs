using System;
using AStar.models.AI.PathFinder;
using AStar.models.IO;

namespace AStar.models.AI.MoveStrategies.MoveStrategies
{
    public class MoveStrategyManager<TCoord> where TCoord : Coords
    {
        private IMoveStrategy<TCoord> MoveStrategy { get; set; }
        private Game Game { get; }

        private IIoManager IoManager { get; }
        public MoveStrategyManager(Game game, IIoManager ioManager)
        {
            Game = game;
            IoManager = ioManager;
        }

        public void UseMoveStrategy<TMoveStrategy>(Func<TMoveStrategy> creator = null) where TMoveStrategy : IMoveStrategy<TCoord>, new()
        {
            MoveStrategy = creator is not null ? creator() : new TMoveStrategy();
        }

        public void MakeMove(TCoord curPlayer, TCoord enemyPlayer, TCoord [][] points)
        {
            MoveStrategy.MakeMove(curPlayer, enemyPlayer, points);
        }
    }
}