using System;
using System.Collections.Generic;
using AI_Model.Pathfinding;
using AI_Model.Utilities;
using CustomMath;

namespace AI_Model.Voronoi
{
    public class Voronoi<NodeType> where NodeType : INode<Vec2Int>, INode, new()
    {
        private Graph<NodeType> graph;

        private Dictionary<NodeType, List<NodeType>> landMarkToNodes = new Dictionary<NodeType, List<NodeType>>();

        private List<NodeType> landmarks = new List<NodeType>();

        public List<NodeType> Landmarks => landmarks;

        public Voronoi(Graph<NodeType> graph)
        {
            this.graph = graph;
        }

        private List<Self_Plane> _planes = new List<Self_Plane>();
        private List<VoronoiPoint<NodeType>> _voronoiObjects;

        public void Bake(ICollection<NodeType> inLandMarks)
        {
            landmarks.Clear();
            landmarks.AddRange(inLandMarks);
            landMarkToNodes.Clear();

            _voronoiObjects = new List<VoronoiPoint<NodeType>>();

            for (int i = 0; i < landmarks.Count; i++)
            {
                _voronoiObjects.Add(new VoronoiPoint<NodeType>());

                foreach (NodeType point in landmarks)
                {
                    if (landmarks[i].Equals(point))
                        continue;

                    Vec3 staticPosition = new Vec3(landmarks[i].GetCoordinate().X, landmarks[i].GetCoordinate().Y, 0.0f);
                    Vec3 pointPosition = new Vec3(point.GetCoordinate().X, point.GetCoordinate().Y, 0.0f);

                    Vec3 dir = new Vec3((staticPosition - pointPosition).normalizedVec3);
                    Vec3 position = Vec3.Lerp(staticPosition, pointPosition, 0.5f);
                    position.x = MathF.Ceiling(position.x);
                    position.y = MathF.Ceiling(position.y);
                    
                    Self_Plane newPlane = new Self_Plane(dir, position);

                    _planes.Add(newPlane);

                    _voronoiObjects[i].planePositions.Add(position);
                    _voronoiObjects[i].planes.Add(newPlane);
                    _voronoiObjects[i].node = landmarks[i];
                }
            }

            foreach (VoronoiPoint<NodeType> point in _voronoiObjects)
            {
                CleanPlanes(point);
            }
        }

        /// <summary>
        /// Expensive Method, only call to draw in debug
        /// </summary>
        public void SetLandmarkNodes()
        {
            foreach (NodeType node in graph.nodes)
            {
                NodeType landmark = GetClosestLandMark(node);

                if (!landMarkToNodes.ContainsKey(landmark))
                    landMarkToNodes.Add(landmark, new List<NodeType>());

                landMarkToNodes[landmark].Add(node);
            }
        }

        private void CleanPlanes(VoronoiPoint<NodeType> voronoiPoint)
        {
            List<Self_Plane> planesToDelete = new List<Self_Plane>();

            for (int i = 0; i < voronoiPoint.planePositions.Count; i++)
            {
                for (int j = 0; j < voronoiPoint.planes.Count; j++)
                {
                    if (i != j)
                        if (!voronoiPoint.planes[j].GetSide(voronoiPoint.planePositions[i]))
                        {
                            planesToDelete.Add(voronoiPoint.planes[i]);
                            break;
                        }
                }
            }

            for (int i = 0; i < planesToDelete.Count; i++)
                voronoiPoint.planes.Remove(planesToDelete[i]);
        }

        public NodeType GetClosestLandMark(NodeType pointNode)
        {
            Vec3 point = new Vec3(pointNode.GetCoordinate().X, pointNode.GetCoordinate().Y, 0.0f);

            bool isPointOut = false;
            foreach (VoronoiPoint<NodeType> voronoiPoint in _voronoiObjects)
            {
                isPointOut = false;
                for (int i = 0; i < voronoiPoint.planes.Count; i++)
                {
                    if (!voronoiPoint.planes[i].GetSide(point))
                    {
                        isPointOut = true;
                    }
                }

                if (isPointOut)
                    continue;

                return voronoiPoint.node;
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