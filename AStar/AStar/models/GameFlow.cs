using System;
using System.Collections.Generic;
using AStar.Controllers;
using AStar.models.AI.MoveStrategies;
using AStar.models.AI.MoveStrategies.BuildStrategies;
using AStar.models.AI.MoveStrategies.MoveStrategies;
using AStar.models.AI.MoveStrategies.WallStrategies;
using AStar.models.IO;

namespace AStar.models
{
    
    public class GameFlow
    {
        private IIoManager IoManager { get; }
        
        private MoveStrategyManager<Point> MoveStrategyManager { get; }
        private bool IsOutMoveFirst { get; set; } = false;
        private Game Game { get; set; }
        private Dictionary<string, IController> Controllers { get; } = new();
        public GameFlow(Game game, IIoManager ioManager, MoveStrategyManager<Point> moveStrategy)
        {
            Game = game;
            IoManager = ioManager;
            MoveStrategyManager = moveStrategy;
        }

        public void UseMoveStrategy<TMoveStrategy>(Func<TMoveStrategy> creator = null) where TMoveStrategy : IMoveStrategy<Point>, new()
        {
            MoveStrategyManager.UseMoveStrategy(creator);
        }

        public void RegisterController(string cmd, IController controller)
        {
            Controllers[cmd] = controller;
        }
        public Point FindBestMove()
        {
            var miniMax = new MiniMax<Point>(3)
                .UseBuildTreeStrategy(() => new QuoridorBuildStrategy(Game, IsOutMoveFirst)); // This mean we are white
            miniMax.BuildTree(Game.Points, Game.CurrentPlayer.GetPosition());
            var res = miniMax.FindBestNode();
            return res.Value;
        }

        public void StartGame()
        {
            Game.PlayWithFriend();
            if (MoveStrategyManager is null)
            {
                UseMoveStrategy(() => new RandomMoveStrategy<Point>(Game, IoManager));
            }
            var side = IoManager.Read()?.Trim();
            IoManager.Write($"// GOT {side}");
            IsOutMoveFirst = side == Constants.White;
            if (side == "") throw new Exception("ddd");
            if (IsOutMoveFirst)
            {
                MoveStrategyManager.MakeMove(Game.CurrentPlayer.GetPosition(), Game.EnemyPlayer.GetPosition(), Game.Points);
            }

            while (!Game.IsGameOver)
            {
                var enemyMove = IoManager.Read();
                var cmds = enemyMove?.Split(" ");
                IoManager.Write($"// Game loop: Got command {enemyMove}");
                if (cmds?.Length != 2)
                {
                    continue;
                }
                Controllers[cmds[0]]?.ExecuteCommand(enemyMove);
                MoveStrategyManager.MakeMove(Game.CurrentPlayer.GetPosition(), Game.EnemyPlayer.GetPosition(), Game.Points);
            }
        }
    }
}