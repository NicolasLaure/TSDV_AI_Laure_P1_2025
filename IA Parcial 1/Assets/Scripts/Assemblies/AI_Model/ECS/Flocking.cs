using System.Collections.Generic;
using CustomMath;

namespace AI_Model.Flocking
{
    public class FlockingManager
    {
        private List<IFlockeable<Vec3>> agents = new List<IFlockeable<Vec3>>();

        private FlockingManager(List<IFlockeable<Vec3>> agents)
        {
            this.agents = agents;
            foreach (IFlockeable<Vec3> agent in agents)
            {
                agent.Init(Alignment, Cohesion, Separation, Direction);
            }
        }

        public Vec3 Alignment(IFlockeable<Vec3> agent)
        {
            var neighbors = GetBoidsInsideRadius(agent);
            if (neighbors.Count == 0)
                return new Vec3(0, 0);

            Vec3 avg = new Vec3(0, 0);
            foreach (IFlockeable<Vec3> b in neighbors)
            {
                avg += b.GetCoordinate();
            }

            return (avg / neighbors.Count).normalizedVec3;
        }

        public Vec3 Cohesion(IFlockeable<Vec3> agent)
        {
            var neighbors = GetBoidsInsideRadius(agent);
            if (neighbors.Count == 0)
                return Vec3.Zero;

            Vec3 avg = Vec3.Zero;
            foreach (IFlockeable<Vec3> b in neighbors)
            {
                avg += b.GetCoordinate();
            }

            avg /= neighbors.Count;
            return (avg - agent.GetCoordinate().normalizedVec3);
        }

        public Vec3 Separation(IFlockeable<Vec3> agent)
        {
            var neighbors = GetBoidsInsideRadius(agent);
            if (neighbors.Count == 0)
                return Vec3.Zero;

            Vec3 avg = Vec3.Zero;
            foreach (IFlockeable<Vec3> b in neighbors)
            {
                avg += agent.GetCoordinate() - b.GetCoordinate();
            }

            return (avg / neighbors.Count).normalizedVec3;
        }

        public Vec3 Direction(IFlockeable<Vec3> agent)
        {
            return agent.GetDir().normalizedVec3;
        }

        public List<IFlockeable<Vec3>> GetBoidsInsideRadius(IFlockeable<Vec3> agent)
        {
            List<IFlockeable<Vec3>> insideRadiusBoids = new List<IFlockeable<Vec3>>();

            foreach (IFlockeable<Vec3> b in agents)
            {
                if (Vec3.Distance(agent.GetCoordinate(), b.GetCoordinate()) < agent.GetDetectionRadius())
                {
                    insideRadiusBoids.Add(b);
                }
            }

            return insideRadiusBoids;
        }
    }
}