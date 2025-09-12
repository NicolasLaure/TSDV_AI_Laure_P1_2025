using System;
using System.Collections.Generic;
using AI_Model.Utilities;

namespace AI_Model.Pathfinding
{
    public class BreadthFirstPathfinder<NodeType> : Pathfinder<NodeType>
        where NodeType : INode<Vec2Int>, INode, new()

    {
        protected override int Distance(NodeType A, NodeType B)
        {
            throw new NotImplementedException();
        }

        public override List<NodeType> FindPath(NodeType startNode, NodeType destinationNode)
        {
            throw new NotImplementedException();
        }

        protected override List<NodeType> GetValidNeighbours(NodeType node)
        {
            throw new NotImplementedException();
        }

        protected override int MoveToNeighborCost(NodeType A, NodeType b)
        {
            throw new NotImplementedException();
        }

        protected override bool NodesEquals(NodeType A, NodeType B)
        {
            throw new NotImplementedException();
        }
    }
}