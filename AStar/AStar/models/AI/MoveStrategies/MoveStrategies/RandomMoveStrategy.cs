using System;
using System.Collections.Generic;
using AStar.models.AI.MoveStrategies.BuildStrategies;
using AStar.models.AI.MoveStrategies.WallStrategies;
using AStar.models.AI.PathFinder;
using AStar.models.IO;

namespace AStar.models.AI.MoveStrategies.MoveStrategies
{
    public class RandomMoveStrategy<TCoord> : IMoveStrategy<TCoord> where TCoord : Coords, new()
    {
        private Game Game { get; set; }
        private IIoManager IoManager { get; set;  }

        private List<Wall> UsedWalls { get; } = new();

        public RandomMoveStrategy()
        {
        }

        public RandomMoveStrategy(Game game, IIoManager ioManager)
        {
            Game = game;
            IoManager = ioManager;
            Game.OnWallPlaced += wall => UsedWalls.Add(wall);
        }

        public void MakeMove(TCoord curPlayer, TCoord enemyPlayer, TCoord[][] points)
        {
            var move = FindBestMove();
            var wall = FindBestWall();
            IoManager.Write($"// Get Point: {move}");
            IoManager.Write($"// MakeMove: {new Move(move).AsString}, Current Player: {Game.CurrentPlayer.Name}");
            if (wall is not null && !UsedWalls.Contains(wall) && Game.CanWallBePlaced(wall))
            {
                IoManager.Write($"// Found wall. Placing {wall}");
                Game.PlaceWall(wall);
                IoManager.Write($"wall {wall.AsString}");
                UsedWalls.Add(wall);
                return;
            }
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
        
        public Point FindBestMove()
        {
            var miniMax = new MiniMax<Point>(3)
                .UseBuildTreeStrategy(() => new QuoridorBuildStrategy(Game, Game.CurrentPlayer.PlayColor == PlayColor.White)); // This mean we are white
            miniMax.BuildTree(Game.Points, Game.CurrentPlayer.GetPosition());
            var res = miniMax.FindBestNode();
            return res.Value;
        }

        public Wall FindBestWall()
        {
            var wallPlace = new WallStreetStrategy<Point>(Game);
            return wallPlace.GetWallToPlace<PlayerMoveDirWallStrategy>(Game.CurrentPlayer.GetPosition(), Game.EnemyPlayer.GetPosition(), Game.Points);
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
    }
}