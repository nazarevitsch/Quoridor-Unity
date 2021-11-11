using System;
using System.Collections.Generic;
using AStar.models.AI.MoveStrategies.BuildStrategies;
using AStar.models.AI.MoveStrategies.WallStrategies;
using AStar.models.AI.PathFinder;
using AStar.models.IO;

namespace AStar.models.AI.MoveStrategies.MoveStrategies
{
    public class PressAgainstTheWall<TCoord> : IMoveStrategy<TCoord> where TCoord : Coords, new()
    {
        private Game Game { get;  }
        private IIoManager IoManager  { get; }

        private List<Wall> UsedWalls { get; } = new();

        public PressAgainstTheWall()
        {
        }

        public PressAgainstTheWall(Game game, IIoManager ioManager)
        {
            Game = game;
            IoManager = ioManager;
            Game.OnWallPlaced += wall => UsedWalls.Add(wall);
        } 
        public void MakeMove(TCoord curPlayer, TCoord enemyPlayer, TCoord[][] points)
        {
            IoManager.Write("// PressAWall_Make_move");
            if (!UsedWalls.Contains(new Wall("w1h")) && enemyPlayer.X == 8 && enemyPlayer.Y == 0)
            {
                var wall = new Wall("w1h");
                Game.PlaceWall(wall);
                IoManager.Write($"wall {wall.AsString}");
                UsedWalls.Add(wall);
                return;
            }

            var curPlayerNode = FindBestMove(Game.CurrentPlayer.GetPosition(), Game.CurrentPlayer.PlayColor == PlayColor.White);
            var enemyPlayerNode = FindBestMove(Game.EnemyPlayer.GetPosition(), Game.EnemyPlayer.PlayColor == PlayColor.White);
            if (enemyPlayerNode.H < curPlayerNode.H)
            {
                IoManager.Write($"// PressAWall_Make_move enemyPlayerNode.H <= curPlayerNode.H ({enemyPlayerNode.H} < {curPlayerNode.H})");
                var wall = new PlayerMoveDirWallStrategy(Game.EnemyPlayer.PlayColor == PlayColor.White ? MoveDirection.Up : MoveDirection.Down).GetWall(curPlayer, enemyPlayer, points);
                if (wall is not null && Game.CanWallBePlaced(wall) && !UsedWalls.Contains(wall))
                {
                    IoManager.Write($"// PressAWall_Make_move !UsedWalls.Contains(wall) ({!UsedWalls.Contains(wall)})");
                    Game.PlaceWall(wall);
                    IoManager.Write($"// PressAWall: wall placed {wall}");
                    IoManager.Write($"wall {wall.AsString}");
                    UsedWalls.Add(wall);
                }
                else
                {
                    DoStep(curPlayerNode.Value);
                }
            }
            else
            {
                DoStep(curPlayerNode.Value);
            }
        }

        private void DoStep(Point move)
        {
            IoManager.Write($"// PressAWall_DoStep: {move} Player is white: {Game.CurrentPlayer.PlayColor == PlayColor.White}");
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

        private MiniMaxNode<Point> FindBestMove(Point point, bool up)
        {
            IoManager.Write($"// Press_Ag_wall move up: {up}, point is {point}");
            var miniMax = new MiniMax<Point>(3)
                .UseBuildTreeStrategy(() => new QuoridorBuildStrategy(Game, up));
            miniMax.BuildTree(Game.Points, point);
            var res = miniMax.FindBestNodeBF();
            return res;
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