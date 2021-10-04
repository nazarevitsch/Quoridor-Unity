using UnityEngine;

namespace models
{
    public class Point
    {
        public GameObject GameObject;
        public bool IsVisited;
        public int X { get; }
        public int Y { get; }

        public Point(GameObject GameObject, int x, int y)
        {
            this.GameObject = GameObject;
            X = x;
            Y = y;
        }
    }
}