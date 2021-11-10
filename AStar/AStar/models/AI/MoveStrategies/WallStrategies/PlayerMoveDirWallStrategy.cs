using AStar.models.AI.PathFinder;

namespace AStar.models.AI.MoveStrategies.WallStrategies
{
    public class PlayerMoveDirWallStrategy : IWallStrategy
    {
        public MoveDirection MoveDirection { get; set; } = MoveDirection.Up;

        public PlayerMoveDirWallStrategy()
        {
        }

        public PlayerMoveDirWallStrategy(MoveDirection moveDirection)
        {
            MoveDirection = moveDirection;
        }

        public Wall GetWall<TCoord>(TCoord curPlayer, TCoord enemyPlayer, TCoord[][] points) where TCoord : Coords
        {
            return new(new Point
            {
                X = enemyPlayer.X - 1,
                Y = MoveDirection == MoveDirection.Down ? enemyPlayer.Y + 1 : enemyPlayer.Y - 1
            }, WallType.Horizontal);
        }
    }
}