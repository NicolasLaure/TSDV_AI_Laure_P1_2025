using System.Collections.Generic;
using AI_Model.Pathfinding;
using AI_Model.Utilities;

namespace AI_Model.Voronoi
{
    public class Voronoi<NodeType> where NodeType : INode<Vec2Int>, INode, IWeightedNode, new()
    {
        private Graph<NodeType> graph;

        private Dictionary<NodeType, List<NodeType>> landMarkToNodes = new Dictionary<NodeType, List<NodeType>>();

        private List<NodeType> landmarks = new List<NodeType>();

        public List<NodeType> Landmarks => landmarks;

        public Voronoi(Graph<NodeType> graph)
        {
            this.graph = graph;
        }

        public void Bake(ICollection<NodeType> inLandMarks)
        {
            landmarks.Clear();
            landmarks.AddRange(inLandMarks);
            landMarkToNodes.Clear();
            Dictionary<NodeType, List<NodeType>> landMarkToOpenNodes = new Dictionary<NodeType, List<NodeType>>();
            Dictionary<NodeType, int> nodes = new Dictionary<NodeType, int>();

            foreach (NodeType node in graph.nodes)
                nodes.Add(node, -1);

            foreach (NodeType node in inLandMarks)
            {
                landMarkToNodes.Add(node, new List<NodeType>());
                landMarkToOpenNodes.Add(node, new List<NodeType>());
                landMarkToOpenNodes[node].Add(node);
                landMarkToNodes[node].Add(node);
            }

            while (landMarkToOpenNodes.Count > 0)
            {
                List<NodeType> keysToRemove = new List<NodeType>();
                foreach (KeyValuePair<NodeType, List<NodeType>> landMark in landMarkToOpenNodes)
                {
                    List<NodeType> neighbours = new List<NodeType>();
                    foreach (NodeType openNode in landMark.Value)
                    {
                        foreach (NodeType neighbour in graph.GetNeighbours(openNode))
                        {
                            int distanceToLandmark = graph.Distance(landMark.Key, neighbour);
                            if (nodes[neighbour] == -1 || distanceToLandmark < nodes[neighbour])
                            {
                                TryRemove(landMarkToNodes, neighbour);
                                nodes.Remove(neighbour);
                                nodes.Add(neighbour, distanceToLandmark);
                                landMarkToNodes[landMark.Key].Add(neighbour);
                                neighbours.Add(neighbour);
                            }
                        }
                    }

                    landMark.Value.Clear();
                    landMark.Value.AddRange(neighbours);
                    if (landMark.Value.Count == 0)
                        keysToRemove.Add(landMark.Key);
                }

                foreach (NodeType landMark in keysToRemove)
                {
                    landMarkToOpenNodes.Remove(landMark);
                }
            }
        }

        public NodeType GetClosestLandMark(NodeType point)
        {
            foreach (KeyValuePair<NodeType, List<NodeType>> area in landMarkToNodes)
            {
                if (area.Value.Contains(point))
                    return area.Key;
            }

            return default;
        }

        public List<NodeType> GetLandmarkNodes(NodeType landmark)
        {
            if (landMarkToNodes.ContainsKey(landmark))
                return landMarkToNodes[landmark];

            return null;
        }

        private void TryRemove(Dictionary<NodeType, List<NodeType>> nodes, NodeType node)
        {
            foreach (KeyValuePair<NodeType, List<NodeType>> keyValue in nodes)
            {
                if (keyValue.Value.Contains(node))
                    keyValue.Value.Remove(node);
            }
        }
    }
}