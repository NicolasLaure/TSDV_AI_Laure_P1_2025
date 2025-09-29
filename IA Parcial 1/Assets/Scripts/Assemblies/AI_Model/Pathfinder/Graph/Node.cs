using System;

namespace AI_Model.Pathfinding
{
    public class Node<Coordinate> : INode, INode<Coordinate>
    {
        private Coordinate coordinate;
        private Enum tileType;
        private bool isBlocked = false;
        private int weight = 0;

        public void SetCoordinate(Coordinate coordinate)
        {
            this.coordinate = coordinate;
        }

        public Coordinate GetCoordinate()
        {
            return coordinate;
        }

        public void SetTileType<EnumType>(EnumType type) where EnumType : Enum
        {
            tileType = type;
        }

        public EnumType GetTileType<EnumType>() where EnumType : Enum
        {
            return (EnumType)tileType;
        }

        public bool IsBlocked()
        {
            return isBlocked;
        }

        public void ToggleBlock()
        {
            Logger.Log($"Is Blocked: {isBlocked}");
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