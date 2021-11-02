namespace AStar.models
{
    public class Player
    {
        public string Name { get; }
        public int StartY;
        public int FinishY;
        public int CurrentX;
        public int CurrentY;
        public int BlocksCount;
        
        public Player(int startY, int finishY, int currentX, int currentY, int blocksCount, string name)
        {
            CurrentX = currentX;
            CurrentY = currentY;
            FinishY = finishY;
            StartY = startY;
            BlocksCount = blocksCount;
            Name = name;
        }

        public Player(int curX, int curY)
        {
            CurrentX = curX;
            CurrentY = curY;
        }
        
        public Point GetPosition()
        {
            return new Point
            {
                X = CurrentX,
                Y = CurrentY
            };
        }
    }
}