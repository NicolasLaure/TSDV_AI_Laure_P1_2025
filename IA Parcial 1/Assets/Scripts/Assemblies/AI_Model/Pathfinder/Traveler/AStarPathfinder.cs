using System;
using System.Collections.Generic;
using UnityEngine;

namespace AI_Model.Pathfinding
{
    public class AStarPathfinder<NodeType> : Pathfinder<NodeType>
        where NodeType : INode<Vector2Int>, INode, IWeightedNode, new()
    {
        private List<NodeType> closedNodes = new List<NodeType>();

        public AStarPathfinder(Graph<NodeType> graph)
        {
            this.graph = graph;
        }

        public class WeightedNode<NodeType> where NodeType : INode<Vector2Int>, INode, IWeightedNode, new()
        {
            public NodeType node;
            public WeightedNode<NodeType> parent;
            public int acumulativeCost;

            public WeightedNode(NodeType node, WeightedNode<NodeType> parent, int acumulativeCost)
            {
                this.node = node;
                this.parent = parent;
                this.acumulativeCost = acumulativeCost;
            }
        }

        public override List<NodeType> FindPath(NodeType startNode, NodeType destinationNode)
        {
            Dictionary<NodeType, WeightedNode<NodeType>> openNodes = new Dictionary<NodeType, WeightedNode<NodeType>>();

            Debug.Log($"StartNode: {startNode.GetCoordinate()}, Target: {destinationNode.GetCoordinate()}");
            NodeType currentNode = startNode;
            closedNodes.Clear();
            WeightedNode<NodeType> initialWNode = new WeightedNode<NodeType>(currentNode, null, 0);
            openNodes.Add(currentNode, initialWNode);
            while (!NodesEquals(currentNode, destinationNode) && openNodes.Count > 0)
            {
                foreach (NodeType node in GetValidNeighbours(currentNode))
                {
                    int acumulativeCost = 0;
                    WeightedNode<NodeType> parent = null;
                    if (openNodes.ContainsKey(currentNode))
                    {
                        acumulativeCost = openNodes[currentNode].acumulativeCost;
                        parent = openNodes[currentNode];
                    }

                    int heuristic = Distance(node, destinationNode) + node.GetWeight();

                    WeightedNode<NodeType> wNode =
                        new WeightedNode<NodeType>(node, parent, acumulativeCost + heuristic);

                    if (!openNodes.ContainsKey(node))
                        openNodes.Add(node, wNode);
                    else if (openNodes[node].acumulativeCost > acumulativeCost + heuristic)
                    {
                        openNodes.Remove(node);
                        openNodes.Add(node, wNode);
                    }
                }

                openNodes.Remove(currentNode);
                closedNodes.Add(currentNode);
                NodeType lessExpensive = new NodeType();
                foreach (NodeType node in openNodes.Keys)
                {
                    if (!openNodes.ContainsKey(lessExpensive))
                    {
                        lessExpensive = node;
                        continue;
                    }

                    if (openNodes[node].acumulativeCost < openNodes[lessExpensive].acumulativeCost)
                        lessExpensive = node;
                }

                currentNode = lessExpensive;
            }

            if (destinationNode.IsBlocked() || !graph.nodes.Contains(currentNode))
            {
                Debug.LogError("No Valid Path");
                return new List<NodeType>();
            }

            return GetPath(openNodes[currentNode]);
        }

        private List<NodeType> GetPath(WeightedNode<NodeType> lastNode)
        {
            List<NodeType> path = new List<NodeType>();
            WeightedNode<NodeType> currentNode = lastNode;
            path.Add(currentNode.node);
            while (currentNode.parent != null)
            {
                currentNode = currentNode.parent;
                path.Add(currentNode.node);
            }

            path.Reverse();
            string pathString = "Path: ";
            foreach (NodeType node in path)
            {
                pathString += node.GetCoordinate().ToString() + ", ";
            }

            Debug.Log(pathString);

            return path;
        }

        protected override List<NodeType> GetValidNeighbours(NodeType node)
        {
            List<NodeType> validNeighbours = new List<NodeType>();
            foreach (NodeType nodeToCheck in graph.GetNeighbours(node))
            {
                if (!nodeToCheck.IsBlocked() && !closedNodes.Contains(nodeToCheck))
                    validNeighbours.Add(nodeToCheck);
            }

            return validNeighbours;
        }

        protected override int Distance(NodeType A, NodeType B)
        {
            Vector2Int aPos = A.GetCoordinate();
            Vector2Int bPos = B.GetCoordinate();

            return Mathf.Abs(aPos.x - bPos.x) + Mathf.Abs(aPos.y - bPos.y);
        }

        protected override int MoveToNeighborCost(NodeType A, NodeType b)
        {
            throw new NotImplementedException();
        }

        protected override bool NodesEquals(NodeType A, NodeType B)
        {
            return A.GetCoordinate().x == B.GetCoordinate().x && A.GetCoordinate().y == B.GetCoordinate().y;
        }
    }
}