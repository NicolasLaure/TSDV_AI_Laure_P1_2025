using System.Collections.Generic;

namespace AI_Model.Pathfinding
{
    public abstract class Graph<NodeType> where NodeType : INode<UnityEngine.Vector2Int>, INode, new()
    {
        public List<NodeType> nodes = new List<NodeType>();
        public Dictionary<NodeType, List<NodeType>> nodeToNeighbours = new Dictionary<NodeType, List<NodeType>>();

        public abstract void PopulateGraph();

        public List<NodeType> GetNeighbours(NodeType node)
        {
            if (nodeToNeighbours.ContainsKey(node))
                return nodeToNeighbours[node];

            return null;
        }
    }
}