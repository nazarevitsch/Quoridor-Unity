using System.Text.Json.Serialization;
using AStar.models;

namespace AStar.networking.PlayerIO
{
    public class Command
    {
        
        public string Code { get; set; }
        public string Name { get; set; }
        public string Guid { get; set; } 
        public Point Position { get; set; } 
    }
}