using System;
using System.Collections;

namespace AI_Model.Pathfinding
{
    public class Node<Coordinate> : INode, INode<Coordinate>
    {
        private Coordinate coordinate;
        private Enum tileType;
        private bool isBlocked = false;
        private int weight = 0;
        private float latitude = 0;
        private float longitude = 0;

        public void SetCoordinate(Coordinate coordinate)
        {
            this.coordinate = coordinate;
        }

        public Coordinate GetCoordinate()
        {
            return coordinate;
        }

        public void SetLatitude(float lat)
        {
            latitude = lat;
        }

        public float GetLatitude()
        {
            return latitude;
        }

        public void SetLongitude(float lon)
        {
            longitude = lon;
        }

        public float GetLongitude()
        {
            return longitude;
        }

        public void SetTileType<EnumType>(EnumType type) where EnumType : Enum
        {
            tileType = type;
        }

        public EnumType GetTileType<EnumType>() where EnumType : Enum
        {
            return (EnumType)tileType;
        }

        public bool EqualsNode(INode<Coordinate> other)
        {
            return coordinate.Equals(other.GetCoordinate());
        }

        public bool IsBlocked()
        {
            return isBlocked;
        }

        public void ToggleBlock()
        {
            isBlocked = !isBlocked;
        }

        public void SetWeight(int newWeight)
        {
            weight = newWeight;
        }

        public int GetWeight()
        {
            return weight;
        }
    }
}