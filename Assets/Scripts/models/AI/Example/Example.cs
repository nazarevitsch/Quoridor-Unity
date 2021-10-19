using System;
using System.Collections.Generic;
using System.Linq;

namespace models.AI.Example
{
    class PifagorHeuristic : IHeuristicStrategy
    {
        public double H<TCoordObj>(TCoordObj current, TCoordObj end, TCoordObj[][] pointsArr) where TCoordObj : Coords
        {
            return Math.Pow(end.X - current.X, 2d) + Math.Pow(end.Y - current.Y, 2d);
        }
    }
    class EightNeighbours : INeighbourFinder
    {
        public List<TCoords> Find<TCoords>(TCoords current, TCoords[][] coords) where TCoords : Coords, new()
        {
            var sizeY = coords.Length;
            if (sizeY == 0) throw new Exception("Field is empty");
            var sizeX = coords[0].Length;
            return new List<TCoords>
            {
                current.X - 1 < 0 || current.Y - 1 < 0 ? null : new TCoords {X = current.X - 1, Y = current.Y - 1}, // TOP-LEFT
                current.Y - 1 < 0 ? null : new TCoords {X = current.X, Y = current.Y - 1}, // TOP
                current.X + 1 >= sizeX || current.Y - 1 < 0 ? null : new TCoords {X = current.X + 1, Y = current.Y - 1}, // TOP-RIGHT
                current.X - 1 < 0 ? null : new TCoords {X = current.X - 1, Y = current.Y}, // MIDDLE-LEFT
                current.X + 1 >= sizeX ? null : new TCoords {X = current.X + 1, Y = current.Y}, // MIDDLE-RIGHT
                current.X - 1 < 0 || current.Y + 1 >= sizeY ? null : new TCoords {X = current.X - 1, Y = current.Y + 1}, // BOTTOM-LEFT
                current.Y + 1 >= sizeY ? null : new TCoords {X = current.X, Y = current.Y + 1}, // BOTTOM-MIDDLE
                current.X + 1 >= sizeX || current.Y + 1 >= sizeY ? null : new TCoords {X = current.X + 1, Y = current.Y + 1}, // BOTTOM-RIGHT
            }
                .Where(coord => coord != null && (string) coords[coord.Y][coord.X].GetInfo() != "1")
                .ToList();
        }
    }
    class ExamplePoint : Coords
    {
        public string Tag { get; set; }
        public override object GetInfo()
        {
            return Tag;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            ExamplePoint[][] point =
            {
                new[]
                {
                    new ExamplePoint{ X=0,Y=0 },
                    new ExamplePoint{ X=1,Y=0 },
                    new ExamplePoint{ X=2,Y=0,Tag = "1" },
                    new ExamplePoint{ X=3,Y=0},
                    new ExamplePoint{ X=4,Y=0 }
                },
                new[]
                {
                    new ExamplePoint{ X=0,Y=1 },
                    new ExamplePoint{ X=1,Y=1 },
                    new ExamplePoint{ X=2,Y=1,Tag = "1" },
                    new ExamplePoint{ X=3,Y=1 },
                    new ExamplePoint{ X=4,Y=1 }
                },
                new[]
                {
                    new ExamplePoint{ X=0,Y=2 },
                    new ExamplePoint{ X=1,Y=2 },
                    new ExamplePoint{ X=2,Y=2,Tag = "1" },
                    new ExamplePoint{ X=3,Y=2 },
                    new ExamplePoint{ X=4,Y=2 }
                },
                new[]
                {
                    new ExamplePoint{ X=0,Y=3 },
                    new ExamplePoint{ X=1,Y=3 },
                    new ExamplePoint{ X=2,Y=3,Tag = "1" },
                    new ExamplePoint{ X=3,Y=3 },
                    new ExamplePoint{ X=4,Y=3 }
                },
                new[]
                {
                    new ExamplePoint{ X=0,Y=4 },
                    new ExamplePoint{ X=1,Y=4 },
                    new ExamplePoint{ X=2,Y=4 },
                    new ExamplePoint{ X=3,Y=4 },
                    new ExamplePoint{ X=4,Y=4 }
                }
            };
            var aStar = new AStar<ExamplePoint>(point)
                .UseHeuristicStrategy<PifagorHeuristic>()
                .UseNeighbourFindStrategy<EightNeighbours>();
            var start = new ExamplePoint {X = 1, Y = 1};
            var finish = new ExamplePoint {X = 4, Y = 3};
            PrintField(point, start, finish);
            var path = aStar.Resolve(start, finish);
            PrintPath(path);
        }

        static void PrintField(ExamplePoint[][] points, ExamplePoint start, ExamplePoint end)
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
    }
}