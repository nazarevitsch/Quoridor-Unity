using System.Collections.Generic;
using System.Linq;

namespace models.AI
{
    public class AStar<TCoord>
        where TCoord : Coords, new()
    {
        public TCoord [][] Coords { get; private set; }
        private IHeuristicStrategy _strategy;
        private INeighbourFinder _neighbourFinder;
        public AStar(TCoord [][] coords)
        {
            Coords = coords;
        }
        
        public AStar<TCoord> UseHeuristicStrategy<T> () where T : IHeuristicStrategy, new()
        {
            _strategy = new T();
            return this;
        }
        public AStar<TCoord> UseNeighbourFindStrategy<TNeighbourFindStrategy> () where TNeighbourFindStrategy : INeighbourFinder, new()
        {
            _neighbourFinder = new TNeighbourFindStrategy();
            return this;
        }

        public List<Node> Resolve(TCoord start, TCoord end)
        {
            var closeList = new List<Node>(10);
            var openList = new List<Node>(10) {new Node()
            {
                X = start.X,
                Y = start.Y,
                G = 0,
                H = 0,
                F = 0,
                Prev = null
            }};
            while (openList.Count > 0)
            {
                var currentNode = openList[0];
                foreach (var node in openList.Where(node => node.F < currentNode.F))
                {
                    currentNode = node;
                }

                openList.Remove(currentNode);
                closeList.Add(currentNode);
                if (currentNode.X == end.X && currentNode.Y == end.Y)
                {
                    var res = new List<Node>();
                    while (currentNode != null)
                    {
                        res.Add(currentNode);
                        currentNode = currentNode.Prev;
                    }
                    res.Reverse();
                    return res;
                }

                var neighbours = FindNearestNodes(currentNode);
                foreach (var neighbour in neighbours)
                {
                    var isExistInCloseList = closeList.Find(node => node.X == neighbour.X && node.Y == neighbour.Y) != null;
                    if (isExistInCloseList) continue;
                    var h = _strategy.H(neighbour, end, Coords);
                    var node1 = new Node
                    {
                        X = neighbour.X,
                        Y = neighbour.Y,
                        H = h,
                        G = 1 + currentNode.G,
                        F = 1 + currentNode.G + h,
                        Prev = currentNode
                    };
                    var nodeInOpenList = openList.Find(node => node.X == neighbour.X && neighbour.Y == node.Y);
                    if (nodeInOpenList is null)
                    {
                        openList.Add(node1);
                    }
                    else if (node1.G < currentNode.G)
                    {
                        nodeInOpenList.Prev = currentNode;
                        nodeInOpenList.G = 1 + currentNode.G;
                        nodeInOpenList.F = 1 + currentNode.G + h;
                    }

                }
            }
            return new List<Node>();
        }

        private List<TCoord> FindNearestNodes(Node currentNode)
        {
            return _neighbourFinder.Find(new TCoord{ X = currentNode.X, Y = currentNode.Y} ,Coords);
        }
    }

    

    public class Node
    {
        public double F { get; set; } // F = G + H
        public double G { get; set; } // G = Size of node + Prev G
        public double H { get; set; } // H = Heuristic Function
        public int X { get; set; } // Coord X
        public int Y { get; set; } // Coord Y
        public Node Prev { get; set; } 
    }
}