using System.Collections.Generic;
using System.Linq;
using AStar.models.AI.PathFinder;
using AStar.models.AI.PathFinder.Euristics;
using AStar.models.AI.PathFinder.PathFindStrategies;

namespace AStar.models.AI.MoveStrategies.BuildStrategies
{
    public class QuoridorBuildStrategy : IBuildStrategy
    {
        private readonly Game _game;
        private readonly bool _moveUp;
        public QuoridorBuildStrategy(Game game, bool moveUp)
        {
            _game = game;
            _moveUp = moveUp;
        }
        public List<TCoords> FindCoordsFroBuildingNodes<TCoords>(TCoords[][] coordsArray, TCoords coord)
            where TCoords : Coords, new()
        {
            return _game
                .FindPossiblePlatformsForWay()
                .Select(point => 
                new TCoords
                {
                    X = point.X,
                    Y = point.Y
                }).ToList();
        }

        public double H <TCoords>(TCoords[][] coordsArray, TCoords coord)  where TCoords : Coords, new()
        {
            var aStar = new AStar<TCoords>()
                .UseHeuristicStrategy<PifagorHeuristic>()
                .UseNeighbourFindStrategy<FourWayPath>();
            var xs = new List<int>
            {
                0, 2, 4, 6, 8, 10, 12, 14, 16 
            //  a, b, c, d, e, f,  g,  h,  i
            };

            return xs.Select(X =>
            {
                return aStar.Resolve(coordsArray, coord, new TCoords
                {
                    X = X,
                    Y = _moveUp ? 0 : 16 // first or last row
                }).Count;
            }
            ).Min();
        }
    }
}