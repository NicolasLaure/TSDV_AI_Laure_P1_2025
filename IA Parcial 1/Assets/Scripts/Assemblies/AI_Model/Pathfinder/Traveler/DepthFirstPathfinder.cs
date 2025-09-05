using System;
using System.Collections.Generic;
using UnityEngine;
using Vector2 = System.Numerics.Vector2;

namespace Pathfinder
{
    public class DepthFirstPathfinder<NodeType> : Pathfinder<NodeType> where NodeType : INode<UnityEngine.Vector2Int>, INode, new()
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
            Vector2Int aPos = A.GetCoordinate();
            Vector2Int bPos = B.GetCoordinate();
            return (int)MathF.Floor(MathF.Abs(aPos.x - bPos.x) + MathF.Abs(aPos.y - bPos.y));
        }

        protected override int MoveToNeighborCost(NodeType A, NodeType b)
        {
            throw new System.NotImplementedException();
        }

        protected override bool NodesEquals(NodeType A, NodeType B)
        {
            return A.GetCoordinate() == B.GetCoordinate();
        }
    }
}