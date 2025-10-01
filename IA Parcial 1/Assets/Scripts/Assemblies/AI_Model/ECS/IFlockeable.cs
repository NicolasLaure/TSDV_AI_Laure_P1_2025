using System;

namespace AI_Model.Flocking
{
    public interface IFlockeable<Coordinate>
    {
        public void Init(Func<IFlockeable<Coordinate>, Coordinate> Alignment,
        Func<IFlockeable<Coordinate>, Coordinate> Cohesion,
        Func<IFlockeable<Coordinate>, Coordinate> Separation,
        Func<IFlockeable<Coordinate>, Coordinate> Direction);

        public Coordinate ACS();
        public Coordinate GetCoordinate();
        public Coordinate GetDir();
        public float GetDetectionRadius();
    }
}