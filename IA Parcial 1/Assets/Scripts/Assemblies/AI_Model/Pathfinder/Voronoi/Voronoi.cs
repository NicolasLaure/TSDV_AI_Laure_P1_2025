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
        public List<VoronoiPoint<NodeType>> voronoiObjects;

        public void Bake(ICollection<NodeType> inLandMarks)
        {
            landmarks.Clear();
            landmarks.AddRange(inLandMarks);
            landMarkToNodes.Clear();

            voronoiObjects = new List<VoronoiPoint<NodeType>>();

            for (int i = 0; i < landmarks.Count; i++)
            {
                voronoiObjects.Add(new VoronoiPoint<NodeType>());

                MyQuaternion mineRotation = MyQuaternion.Euler(landmarks[i].GetLatitude(), landmarks[i].GetLongitude(), 0.0f).normalized;

                foreach (NodeType point in landmarks)
                {
                    if (landmarks[i].GetCoordinate() == point.GetCoordinate())
                        continue;

                    MyQuaternion otherRot = MyQuaternion.Euler(point.GetLatitude(), point.GetLongitude(), 0.0f).normalized;

                    MyQuaternion halfRot = MyQuaternion.Slerp(mineRotation, otherRot, 0.5f).normalized;
                    Vec3 halfPoint = (halfRot * Vec3.Up).normalizedVec3;

                    Vec3 minePoint = (mineRotation * Vec3.Up).normalizedVec3;

                    Vec3 otherPoint = (otherRot * Vec3.Up).normalizedVec3;

                    Vec3 firstCross = Vec3.Cross(minePoint, otherPoint).normalizedVec3;
                    Vec3 planeNormal = Vec3.Cross(firstCross, halfPoint).normalizedVec3;
                    Self_Plane plane = new Self_Plane(planeNormal, 0);
                    voronoiObjects[i].planes.Add(plane);
                    voronoiObjects[i].node = landmarks[i];
                }
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
            Vec3 point = MyQuaternion.Euler(pointNode.GetLatitude(),pointNode.GetLongitude(),0.0f) * Vec3.Up;

            bool isPointOut = false;
            foreach (VoronoiPoint<NodeType> voronoiPoint in voronoiObjects)
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