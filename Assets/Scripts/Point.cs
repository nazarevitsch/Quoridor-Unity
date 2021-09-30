using UnityEngine;

namespace DefaultNamespace
{
    public class Point
    {
        public GameObject GameObject;
        public bool IsVisited;

        public Point(GameObject GameObject)
        {
            this.GameObject = GameObject;
        }
    }
}