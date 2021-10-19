using System.Collections.Generic;

namespace models.AI
{
    public interface INeighbourFinder
    {
       List<TCoords> Find<TCoords>(TCoords current,TCoords[][] coords) where TCoords : Coords, new();
    }
}