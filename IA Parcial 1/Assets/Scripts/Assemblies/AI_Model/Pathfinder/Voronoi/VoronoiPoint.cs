using System.Collections.Generic;
using AI_Model.Pathfinding;
using AI_Model.Utilities;
using CustomMath;

namespace AI_Model.Voronoi
{
    public class VoronoiPoint<NodeType> where NodeType : INode<Vec2Int>, INode, new()
    {
        public List<Self_Plane> planes = new List<Self_Plane>();
        public List<Vec3> planePositions = new List<Vec3>();
        public NodeType node;
    }
}