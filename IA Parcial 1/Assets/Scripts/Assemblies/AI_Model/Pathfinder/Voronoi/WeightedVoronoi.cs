using System.Collections.Generic;
using AI_Model.Pathfinding;
using AI_Model.Utilities;
using CustomMath;

namespace AI_Model.Voronoi
{
    public class WeightedVoronoi<NodeType> where NodeType : INode<Vec2Int>, INode, new()
    {
        private Graph<NodeType> graph;

        private Dictionary<NodeType, List<NodeType>> landMarkToNodes = new Dictionary<NodeType, List<NodeType>>();

        private List<NodeType> landmarks = new List<NodeType>();

        private Dictionary<int, Transitability> typeToWeight;
        public List<NodeType> Landmarks => landmarks;

        public WeightedVoronoi(Graph<NodeType> graph, Dictionary<int, Transitability> typeToWeight)
        {
            this.graph = graph;
            this.typeToWeight = typeToWeight;
        }

        public List<VoronoiPoint<NodeType>> voronoiObjects = new List<VoronoiPoint<NodeType>>();

        public void Bake(ICollection<NodeType> inLandMarks)
        {
            landmarks.Clear();
            landmarks.AddRange(inLandMarks);
            landMarkToNodes.Clear();
            voronoiObjects.Clear();

            List<NodeType> checkedNodes = new List<NodeType>();

            for (int i = 0; i < landmarks.Count; i++)
            {
                voronoiObjects.Add(new VoronoiPoint<NodeType>());
            }

            for (int i = 0; i < landmarks.Count; i++)
            {
                voronoiObjects[i].node = landmarks[i];

                MyQuaternion mineRotation = MyQuaternion.Euler(landmarks[i].GetLatitude(), landmarks[i].GetLongitude(), 0.0f).normalized;

                for (int j = 0; j < landmarks.Count; j++)
                {
                    if (i == j || checkedNodes.Contains(landmarks[j]))
                        continue;

                    MyQuaternion otherRot = MyQuaternion.Euler(landmarks[j].GetLatitude(), landmarks[j].GetLongitude(), 0.0f).normalized;

                    MyQuaternion halfRot = MyQuaternion.Slerp(mineRotation, otherRot, 0.5f).normalized;

                    Vec3 halfPoint = (halfRot * Vec3.Up).normalizedVec3;

                    Vec3 minePoint = (mineRotation * Vec3.Up).normalizedVec3;

                    Vec3 otherPoint = (otherRot * Vec3.Up).normalizedVec3;

                    Vec3 firstCross = Vec3.Cross(minePoint, otherPoint).normalizedVec3;
                    Vec3 planeNormal = Vec3.Cross(halfPoint, firstCross).normalizedVec3;

                    Self_Plane plane = new Self_Plane(planeNormal, 0);
                    Self_Plane otherPlane = new Self_Plane(planeNormal, 0);
                    otherPlane.Flip();

                    voronoiObjects[i].planes.Add(plane);
                    voronoiObjects[i].planePositions.Add(halfRot * Vec3.Up);

                    voronoiObjects[j].planes.Add(otherPlane);
                    voronoiObjects[j].planePositions.Add(halfRot * Vec3.Up);

                    Bsp(plane, otherPlane, minePoint, otherPoint, 0, 2);
                    checkedNodes.Add(landmarks[i]);
                }

                CleanPlanes(voronoiObjects[i]);
            }
        }

        private void Bsp(Self_Plane plane, Self_Plane otherPlane, Vec3 dir, Vec3 otherDir, float currentDisplace, int steps)
        {
            if (steps <= 0)
                return;

            int firstHalfWeight = 0;
            int secondHalfWeight = 0;

            float dot = Vec3.Dot(plane.Normal, dir);
            float otherDot = Vec3.Dot(plane.Normal, otherDir);

            foreach (NodeType node in graph.nodes)
            {
                if (plane.GetSide(MyQuaternion.Euler(node.GetLatitude(), node.GetLongitude(), 0) * Vec3.Up))
                    firstHalfWeight += typeToWeight[node.GetTileType()].weight;
                else
                    secondHalfWeight += typeToWeight[node.GetTileType()].weight;
            }

            if (firstHalfWeight == secondHalfWeight)
                return;

            float newDisplace = currentDisplace;
            if (firstHalfWeight > secondHalfWeight)
                newDisplace += (dot - currentDisplace) / 2;
            else
                newDisplace += (otherDot - currentDisplace) / 2;

            plane.SetDistance(newDisplace);
            otherPlane.SetDistance(-newDisplace);

            Bsp(plane, otherPlane, dir, otherDir, newDisplace, steps - 1);
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
            if (voronoiObjects.Count == 1)
                return voronoiObjects[0].node;

            Vec3 point = MyQuaternion.Euler(pointNode.GetLatitude(), pointNode.GetLongitude(), 0.0f).normalized * Vec3.Up;
            point.Normalize();

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