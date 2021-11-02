using System.Collections.Generic;

namespace AStar.models.AI.PathFinder
{
    public interface IPathFinder
    {
        public List<TCoords> Find<TCoords>(TCoords current,TCoords[][] coords) where TCoords : Coords, new();
    }
}