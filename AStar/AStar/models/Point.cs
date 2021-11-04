﻿using System;
using AStar.models.AI;
using AStar.models.AI.PathFinder;

namespace AStar.models
{
    public class Point : Coords
    {
        public bool IsVisited;
        public string Tag { get; set; } = "0";

        private string Str => new Move(this).AsString;

        public Point(int x, int y, string tag = "")
        {
            X = x;
            Y = y;
            Tag = tag;
        }

        public Point()
        {
        }

        public override object GetInfo()
        {
            return Tag;
        }
        
        public override string ToString()
        {
            return $"Point(X={X}, Y={Y}, str={Str})";
        }
    }
}