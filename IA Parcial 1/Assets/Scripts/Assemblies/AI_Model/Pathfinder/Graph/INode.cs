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
    }

    public interface IWeightedNode
    {
        public void SetWeight(int newWeight);
        public int GetWeight();
    }
}