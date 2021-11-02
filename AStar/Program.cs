using System;
using System.Collections.Generic;
using System.Linq;
using AStar.Controllers;
using AStar.models;
using AStar.models.AI.PathFinder;

namespace AStar
{
    
    class Program
    {
        static void PrintField(Point[][] points, Point start, Point end)
        {
            foreach (var row in points)
            {
                foreach (var col in row)
                {
                    if (col.X == start.X && col.Y == start.Y)
                    {
                        Console.Write("* ");
                    }
                    else if (col.X == end.X && col.Y == end.Y)
                    {
                        Console.Write("X "); 
                    }
                    else
                    {
                        Console.Write($"{col.Tag} ");

                    }
                }
                Console.WriteLine();
            }
        }
        static void PrintPath(List<Node> path)
        {
            Console.WriteLine(string.Join("=>", path.Select(node => $"[{node.X}, {node.Y}]")));
        }
        
        static void Main(string[] args)
        {
            var game = new Game();
            var gameFlow = new GameFlow(game);
            Console.WriteLine("// Start game");
            gameFlow.RegisterController("move", new MoveController(game));
            gameFlow.RegisterController("wall", new WallController(game));
            gameFlow.RegisterController("jump", new JumpController(game));
            gameFlow.StartGame();
        }
    }
}