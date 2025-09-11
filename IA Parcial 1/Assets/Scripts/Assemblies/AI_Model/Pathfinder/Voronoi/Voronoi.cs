using System.Collections.Generic;
using AI_Model.Pathfinding;
using UnityEngine;

namespace AI_Model.Voronoi
{
    public class Voronoi<NodeType> where NodeType : INode<Vector2Int>, INode, IWeightedNode, new()
    {
        private Graph<NodeType> graph;

        public Voronoi(Graph<NodeType> graph)
        {
            this.graph = graph;
        }

        public void Bake(ICollection<NodeType> landMarks)
        {
        }
    }
}