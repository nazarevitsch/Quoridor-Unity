namespace models.AI
{
    public interface IHeuristicStrategy
    {
        double H<TCoordObj>(TCoordObj current, TCoordObj end, TCoordObj[][] pointsArr) where TCoordObj : Coords;
    }
}