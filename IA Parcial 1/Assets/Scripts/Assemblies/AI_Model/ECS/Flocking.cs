using System.Collections.Generic;
using AI_Model.Pathfinding;
using AI_Model.Utilities;
using CustomMath;

namespace AI_Model.Flocking
{
    public class Flocking<NodeType> where NodeType : INode<Vec2Int>, INode, new()
    {
        private List<IFlockeable<NodeType>> agents = new List<IFlockeable<NodeType>>();
        private Grid<NodeType> grid;

        public Flocking(List<IFlockeable<NodeType>> agents, Grid<NodeType> grid)
        {
            this.grid = grid;
            this.agents = agents;
            foreach (IFlockeable<NodeType> agent in agents)
            {
                agent.Init(Alignment, Cohesion, Separation, Direction);
            }
        }

        public Vec3 Alignment(IFlockeable<NodeType> agent)
        {
            List<IFlockeable<NodeType>> neighbors = GetBoidsInsideRadius(agent);
            if (neighbors.Count == 0)
                return Vec3.Zero;

            Vec3 avg = Vec3.Zero;
            foreach (IFlockeable<NodeType> b in neighbors)
            {
                avg += b.GetTransform().forward;
            }

            return (avg / neighbors.Count).normalizedVec3;
        }

        public Vec3 Cohesion(IFlockeable<NodeType> agent)
        {
            List<IFlockeable<NodeType>> neighbors = GetBoidsInsideRadius(agent);
            if (neighbors.Count == 0)
                return Vec3.Zero;

            Vec3 avg = Vec3.Zero;
            foreach (IFlockeable<NodeType> b in neighbors)
            {
                avg += b.GetTransform().Position;
            }

            avg /= neighbors.Count;
            return (avg - agent.GetCoordinate()).normalizedVec3;
        }

        public Vec3 Separation(IFlockeable<NodeType> agent)
        {
            List<IFlockeable<NodeType>> neighbors = GetBoidsInsideRadius(agent);

            if (neighbors.Count == 0)
                return Vec3.Zero;

            Vec3 avg = Vec3.Zero;
            foreach (IFlockeable<NodeType> b in neighbors)
            {
                avg += agent.GetTransform().Position - b.GetTransform().Position;
            }

            return (avg / neighbors.Count).normalizedVec3;
        }

        public Vec3 Direction(IFlockeable<NodeType> agent)
        {
            Vec3 position = agent.GetTransform().Position;
            Vec2Int nextPosition = agent.GetNextPosition().GetCoordinate();

            return (new Vec3(nextPosition.X, nextPosition.Y, 0) - position).normalizedVec3;
        }

        public List<IFlockeable<NodeType>> GetBoidsInsideRadius(IFlockeable<NodeType> agent)
        {
            List<IFlockeable<NodeType>> insideRadiusBoids = new List<IFlockeable<NodeType>>();

            foreach (IFlockeable<NodeType> b in agents)
            {
                if (Vec3.Distance(agent.GetTransform().Position, b.GetTransform().Position) < agent.GetDetectionRadius())
                {
                    insideRadiusBoids.Add(b);
                }
            }

            return insideRadiusBoids;
        }
    }
}