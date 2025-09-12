using System.Collections.Generic;
using AI_Model.Utilities;

namespace AI_Model.Pathfinding
{
    public abstract class Graph<NodeType> where NodeType : INode<Vec2Int>, INode, new()
    {
        public List<NodeType> nodes = new List<NodeType>();
        public Dictionary<NodeType, List<NodeType>> nodeToNeighbours = new Dictionary<NodeType, List<NodeType>>();

        public abstract void PopulateGraph();

        public List<NodeType> GetNeighbours(NodeType node)
        {
            if (nodeToNeighbours.ContainsKey(node))
                return nodeToNeighbours[node];

            return new List<NodeType>();
        }

        public abstract int Distance(NodeType A, NodeType B);
    }
}