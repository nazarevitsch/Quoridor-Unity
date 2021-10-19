using System;

namespace models.AI
{
    public abstract class Coords
    {
        public int X { get; set; }
        public int Y { get; set; }
        public abstract Object GetInfo();
    }
}