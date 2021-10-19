namespace models
{
    public class Point
    {
        public bool IsVisited;
        public int X { get; }
        public int Y { get; }
        public string Tag { get; set; }
        public Point(int x, int y, string tag = "")
        {
            X = x;
            Y = y;
            Tag = tag;
        }
    }
}