﻿using System;
using System.Collections.Generic;
using AStar.Controllers;
using AStar.models.AI.MoveStrategies;
using AStar.models.AI.MoveStrategies.BuildStrategies;
namespace AStar.models
{
    
    public class GameFlow
    {
        private bool IsOutMoveFirst { get; set; } = false;
        private Game Game { get; set; }
        private Dictionary<string, IController> Controllers { get; } = new();
        public GameFlow(Game game)
        {
            Game = game;
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
            Console.WriteLine($"// Get Point: {move}");
            Console.WriteLine($"// MakeMove: {new Move(move).AsString}");
            if (IsJump(move))
            {
                Console.WriteLine("// Jump");
                Console.WriteLine("jump " + new Move(move).AsString);
                Game.DoStep(move);
            }
            else
            {
                Game.DoStep(move);
                Console.WriteLine("move " + new Move(move).AsString);
            }
        }

        private bool IsJump(Point point)
        {
            return Math.Abs(point.X - Game.CurrentPlayer.CurrentX) == 4 ||
                   Math.Abs(point.Y - Game.CurrentPlayer.CurrentY) == 4;
        } 
        public void StartGame()
        {
            Game.PlayWithFriend();
            Game.ChangePuttBlockState();
            var side = Console.In.ReadLine()?.Trim();
            Console.WriteLine($"// GOT {side}");
            IsOutMoveFirst = side == Constants.White;
            if (side == "") throw new Exception("ddd");
            if (IsOutMoveFirst)
            {
                MakeMove();
            }

            while (!Game.IsGameOver)
            {
                var enemyMove = Console.ReadLine();
                var cmds = enemyMove?.Split(" ");
                Console.WriteLine($"// Game loop: Got command {enemyMove}");
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