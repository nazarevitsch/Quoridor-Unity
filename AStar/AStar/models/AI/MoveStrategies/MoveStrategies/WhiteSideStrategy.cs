using System.Collections.Generic;
using AStar.models.AI.MoveStrategies.BuildStrategies;
using AStar.models.AI.PathFinder;
using AStar.models.IO;

namespace AStar.models.AI.MoveStrategies.MoveStrategies
{
    public class WhiteSideStrategy<TCoords> : IMoveStrategy<TCoords> where TCoords : Coords
    {
        private Game Game { get; }
        private IIoManager IoManager { get; }

        private List<Wall> UsedWalls = new (5);
        public WhiteSideStrategy()
        {
        }

        public WhiteSideStrategy(Game game, IIoManager ioManager)
        {
            Game = game;
            IoManager = ioManager;
            Game.OnWallPlaced += wall => UsedWalls.Add(wall);
        }
        public void MakeMove(TCoords curPlayer, TCoords enemyPlayer, TCoords[][] points)
        {
            var curPlayerPos = new Move(Game.EnemyPlayer.GetPosition()).AsString;
            if (curPlayerPos == "e1")
            {
                var wall = new Wall("w1h");
                if (UsedWalls.Contains(wall)) return;
                IoManager.Write("// enemy on e1");
                IoManager.Write("wall " + wall.AsString);
                Game.PlaceWall(wall);
            }
            else if (curPlayerPos == "d1")
            {
                var wall = new Wall("v2h");
                if (UsedWalls.Contains(wall)) return;
                IoManager.Write("// enemy on d1");
                IoManager.Write("wall " + wall.AsString);
                Game.PlaceWall(wall);
            }
            else if (curPlayerPos == "d2")
            {
                var move = new Move("e8");
                IoManager.Write("// enemy on d2");
                IoManager.Write("move " + move.AsString);
                Game.DoStep(move.Point);
            }
            else
            {
                var move = FindBestMove();
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
    }
}