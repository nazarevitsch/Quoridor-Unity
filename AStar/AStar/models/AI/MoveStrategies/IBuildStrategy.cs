using System.Collections.Generic;
using AStar.models.AI.PathFinder;

namespace AStar.models.AI.MoveStrategies
{
    public interface IBuildStrategy
    {
        public List<TCoords> FindCoordsFroBuildingNodes<TCoords>(TCoords[][] coordsArray, TCoords coord)
            where TCoords : Coords, new() ;
        
        public double H<TCoords>(TCoords [][] coordsArray, TCoords coord) 
            where TCoords : Coords, new();
    }
}