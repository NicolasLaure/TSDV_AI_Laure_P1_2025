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

        public void SetTileType<EnumType>(EnumType type) where EnumType : Enum;
        public EnumType GetTileType<EnumType>() where EnumType : Enum;

        public bool EqualsNode(INode<Coordinate> other);
    }


    public interface IWeightedNode
    {
        public void SetWeight(int newWeight);
        public int GetWeight();
    }
}