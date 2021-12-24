using models;

namespace Networking
{
    public class Command
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Guid { get; set; } 
        public Point Position { get; set; } 
        public PlayColor PlayColor { get; set; }
    }
}