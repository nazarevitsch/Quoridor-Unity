using System;
using System.Collections.Generic;
using AStar.Controllers;
using AStar.models.AI.MoveStrategies;
using AStar.models.AI.MoveStrategies.BuildStrategies;
using AStar.models.IO;

namespace AStar.models
{
    
    public class GameFlow
    {
        private IIoManager IoManager { get; }
        private bool IsOutMoveFirst { get; set; } = false;
        private Game Game { get; set; }
        private Dictionary<string, IController> Controllers { get; } = new();
        public GameFlow(Game game, IIoManager ioManager)
        {
            Game = game;
            IoManager = ioManager;
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
            var move = res;
            while (move.Parent != miniMax.Root)
            {
                move = move.Parent;
            }
            return move.Value;
        }

        private void MakeMove()
        {
            var move = FindBestMove();
            IoManager.Write($"// Get Point: {move}");
            IoManager.Write($"// MakeMove: {new Move(move).AsString}, Current Player: {Game.CurrentPlayer.Name}");
            if (IsJump(move) || IsPlayer(move))
            {
                Console.WriteLine("// Jump");
                Console.WriteLine("jump " + GetJumpMove(move).AsString);
                Game.DoStep(move);
            }
            else
            {
                Game.DoStep(move);
                IoManager.Write("move " + new Move(move).AsString);
            }
        }

        private Move GetJumpMove(Point point)
        {
            IoManager.Write($"// Get point {point}");
            if (Game.CurrentPlayer.CurrentX != point.X)
            {
                point.X += point.X > Game.CurrentPlayer.CurrentX ? 1 : -1;
                IoManager.Write($"// {point}");
                return new Move(point);
            }
            
            point.Y += point.Y > Game.CurrentPlayer.CurrentY ? 2 : -2;
            IoManager.Write($"// {point}");
            return new Move(point);
        }

        private bool IsPlayer(Point point)
        {
            return point.X == Game.EnemyPlayer.CurrentX && point.Y == Game.EnemyPlayer.CurrentY;
        }

        private bool IsJump(Point point)
        {
            return Math.Abs(point.X - Game.CurrentPlayer.CurrentX) == 4 ||
                   Math.Abs(point.Y - Game.CurrentPlayer.CurrentY) == 4;
        } 
        public void StartGame()
        {
            Game.PlayWithFriend();
            var side = IoManager.Read()?.Trim();
            IoManager.Write($"// GOT {side}");
            IsOutMoveFirst = side == Constants.White;
            if (side == "") throw new Exception("ddd");
            if (IsOutMoveFirst)
            {
                MakeMove();
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
                MakeMove();
            }
        }
    }
}