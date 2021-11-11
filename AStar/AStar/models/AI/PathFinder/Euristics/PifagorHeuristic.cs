using System;

namespace AStar.models.AI.PathFinder.Euristics
{
    public class PifagorHeuristic : IHeuristicStrategy
    {
        public double H<TCoordObj>(TCoordObj current, TCoordObj end, TCoordObj[][] pointsArr) where TCoordObj : Coords
        {
            return Math.Pow(end.X - current.X, 2d) + Math.Pow(end.Y - current.Y, 2d);
        }
    }
}