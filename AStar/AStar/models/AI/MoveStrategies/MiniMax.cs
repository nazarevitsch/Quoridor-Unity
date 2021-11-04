using System;
using System.Collections.Generic;
using System.Linq;
using AStar.models.AI.PathFinder;

namespace AStar.models.AI.MoveStrategies
{
    public enum NodeType
    {
        Min,
        Max
    }

    public class MiniMax<TCoords> where TCoords: Coords, new()
    {
        private int MaxDepth { get; }

        public MiniMaxNode<TCoords> Root { get; set; }
        private IBuildStrategy BuildStrategy { get; set; }

        public MiniMax(int maxDepth)
        {
            MaxDepth = maxDepth;
        }

        public MiniMax<TCoords> UseBuildTreeStrategy<TStrategy>() where TStrategy : IBuildStrategy, new()
        {
            BuildStrategy = new TStrategy();
            return this;
        }
        
        public MiniMax<TCoords> UseBuildTreeStrategy<TStrategy>(Func<TStrategy> creator) where TStrategy : IBuildStrategy
        {
            BuildStrategy = creator();
            return this;
        }

        private void BuildTree(MiniMaxNode<TCoords> parent, TCoords[][] coordsArray, TCoords point, int depth)
        {
            var children = BuildStrategy.FindCoordsFroBuildingNodes(coordsArray, point);
            foreach (var child in children.Where(c => c != point))
            {
                var node = new MiniMaxNode<TCoords>
                {
                    Parent = parent,
                    NodeType = parent.NodeType == NodeType.Min ? NodeType.Max : NodeType.Min,
                    H = BuildStrategy.H(coordsArray, child),
                    Value = child
                };
                parent.Children.Add(node);
                node.Parent = parent;
                if (depth != MaxDepth)
                {
                    BuildTree(node, coordsArray, node.Value, depth + 1);
                }
            }
        }

        public void BuildTree(TCoords[][] coordsArray, TCoords playerPos)
        {
            Root = new MiniMaxNode<TCoords>
            {
                NodeType = NodeType.Max,
                Value = playerPos
            };
            BuildTree(Root, coordsArray, playerPos, 1);
        }

        public MiniMaxNode<TCoords> FindBestNode()
        {
            var best = Max(Root);
            while (best.Parent != Root)
            {
                best = best.Parent;
            }

            return best;
        }

        private MiniMaxNode<TCoords> Max(MiniMaxNode<TCoords> node)
        {
            return node.Children.Count == 0 ? node : node.Children.Select(Min).ToList().Min();
        }

        private MiniMaxNode<TCoords> Min(MiniMaxNode<TCoords> node)
        {
            return node.Children.Count == 0 ? node: node.Children.Select(Max).ToList().Min();
        }

    }
    public class MiniMaxNode<T> : IComparable
    {
        public MiniMaxNode<T> Parent { get; set; }
        public List<MiniMaxNode<T>> Children { get; set; } = new();
        public double H { get; set; } // Heuristic score
        public NodeType NodeType { get; set; }
        public T Value { get; set; }
        public int CompareTo(object obj)
        {
            var otherNode = obj as MiniMaxNode<T>;
            return otherNode is null ? 1 : H.CompareTo(otherNode.H);
        }
    }
}