namespace models
{
    public class GameInstances
    {
        public Point[][] points;
        public Player player1;
        public Player player2;

        public GameInstances(Point[][] points, Player player1, Player player2)
        {
            this.points = points;
            this.player1 = player1;
            this.player2 = player2;
        }
    }
}