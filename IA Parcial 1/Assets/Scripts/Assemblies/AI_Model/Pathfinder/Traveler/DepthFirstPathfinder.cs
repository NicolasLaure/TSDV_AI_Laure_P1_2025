using System;
using System.Collections.Generic;
using AI_Model.Utilities;

namespace AI_Model.Pathfinding
{
    public class DepthFirstPathfinder<NodeType> : Pathfinder<NodeType> where NodeType : INode<Vec2Int>, INode, new()
    {
        public DepthFirstPathfinder(Graph<NodeType> graph)
        {
            this.graph = graph;
        }

        public override List<NodeType> FindPath(NodeType startNode, NodeType destinationNode)
        {
            List<NodeType> blockedNodes = new List<NodeType>();
            List<NodeType> path = new List<NodeType>();
            path.Add(startNode);

            while (!NodesEquals(path[path.Count], destinationNode))
            {
                
            }

            return path;
        }

        protected override List<NodeType> GetValidNeighbours(NodeType node)
        {
            List<NodeType> neighbours = new List<NodeType>();
            foreach (NodeType neighbour in graph.GetNeighbours(node))
            {
                if (!neighbour.IsBlocked())
                    neighbours.Add(neighbour);
            }

            return neighbours;
        }

        protected override int Distance(NodeType A, NodeType B)
        {
            Vec2Int aPos = A.GetCoordinate();
            Vec2Int bPos = B.GetCoordinate();
            return (int)MathF.Floor(MathF.Abs(aPos.X - bPos.X) + MathF.Abs(aPos.Y - bPos.Y));
        }

        protected override int MoveToNeighborCost(NodeType A, NodeType b)
        {
            throw new NotImplementedException();
        }

        protected override bool NodesEquals(NodeType A, NodeType B)
        {
            return A.GetCoordinate() == B.GetCoordinate();
        }
    }
}