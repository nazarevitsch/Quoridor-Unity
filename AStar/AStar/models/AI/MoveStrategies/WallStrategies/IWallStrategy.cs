using AStar.models.AI.PathFinder;

namespace AStar.models.AI.MoveStrategies.WallStrategies
{
    public enum MoveDirection
    {
        Up,
        Down
    }
    public interface IWallStrategy
    {
        public MoveDirection MoveDirection { get; set; }
        public Wall GetWall<TCoord>(TCoord curPlayer, TCoord enemyPlayer, TCoord[][] points) where TCoord : Coords;
    }
}