using System;
using System.Collections.Generic;
using AI_Model.Pathfinding;
using CustomMath;

namespace AI_Model.Flocking
{
    public interface IFlockeable<NodeType> where NodeType : INode   {
        public void Init(Func<IFlockeable<NodeType>, Vec3> Alignment,
            Func<IFlockeable<NodeType>, Vec3> Cohesion,
            Func<IFlockeable<NodeType>, Vec3> Separation,
            Func<IFlockeable<NodeType>, Vec3> Direction);

        public Vec3 ACS();
        public Vec3 GetCoordinate();
        public List<NodeType> GetPath();
        public MyTransform GetTransform();
        public NodeType GetNextPosition();
        public float GetDetectionRadius();
    }
}