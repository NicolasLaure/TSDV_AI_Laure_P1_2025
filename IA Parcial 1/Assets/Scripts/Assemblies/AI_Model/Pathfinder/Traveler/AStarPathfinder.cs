using System;
using System.Collections.Generic;
using AI_Model.Utilities;

namespace AI_Model.Pathfinding
{
    public class AStarPathfinder<NodeType> : Pathfinder<NodeType>
        where NodeType : INode<Vec2Int>, INode, new()
    {
        private List<NodeType> closedNodes = new List<NodeType>();

        private Dictionary<int, Transitability> typeToWeight;

        public AStarPathfinder(Graph<NodeType> graph, Dictionary<int, Transitability> typeToWeight)
        {
            this.graph = graph;
            this.typeToWeight = typeToWeight;
        }

        public class WeightedNode<NodeType> where NodeType : INode<Vec2Int>, INode, new()
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
            if (startNode == null || destinationNode == null)
                throw new Exception("Received invalid Node");

            Dictionary<NodeType, WeightedNode<NodeType>> openNodes = new Dictionary<NodeType, WeightedNode<NodeType>>();

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

                    int heuristic = Distance(node, destinationNode) + typeToWeight[node.GetTileType()].weight;

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
            return path;
        }

        protected override List<NodeType> GetValidNeighbours(NodeType node)
        {
            List<NodeType> validNeighbours = new List<NodeType>();
            foreach (NodeType nodeToCheck in graph.GetNeighbours(node))
            {
                if (!nodeToCheck.IsBlocked() && typeToWeight[nodeToCheck.GetTileType()].isWalkable && !closedNodes.Contains(nodeToCheck))
                    validNeighbours.Add(nodeToCheck);
            }

            return validNeighbours;
        }

        protected override int Distance(NodeType A, NodeType B)
        {
            Vec2Int aPos = A.GetCoordinate();
            Vec2Int bPos = B.GetCoordinate();

            return (int)MathF.Abs(aPos.X - bPos.X) + (int)MathF.Abs(aPos.Y - bPos.Y);
        }

        protected override int MoveToNeighborCost(NodeType A, NodeType b)
        {
            throw new NotImplementedException();
        }

        protected override bool NodesEquals(NodeType A, NodeType B)
        {
            return A.GetCoordinate().X == B.GetCoordinate().X && A.GetCoordinate().Y == B.GetCoordinate().Y;
        }
    }
}