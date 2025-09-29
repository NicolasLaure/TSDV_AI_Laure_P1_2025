using System.Collections.Generic;
using AI_Model.Utilities;

namespace AI_Model.Pathfinding
{
    public class Path<NodeType> where NodeType : INode<Vec2Int>, INode, new()
    {
        public List<NodeType> nodes = new List<NodeType>();
    }
}