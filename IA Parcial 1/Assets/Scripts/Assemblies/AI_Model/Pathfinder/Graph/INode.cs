using System;

namespace AI_Model.Pathfinding
{
    public interface INode
    {
        public bool IsBlocked();
        public void ToggleBlock();
    }

    public interface INode<Coordinate>
    {
        public void SetCoordinate(Coordinate coordinateType);
        public Coordinate GetCoordinate();

        public void SetLatitude(float lat);
        public float GetLatitude();
        public void SetLongitude(float lon);
        public float GetLongitude();

        public void SetTileType(int type);
        public int GetTileType();

        public bool EqualsNode(INode<Coordinate> other);
    }


    public interface IWeightedNode
    {
        public void SetWeight(int newWeight);
        public int GetWeight();
    }
}