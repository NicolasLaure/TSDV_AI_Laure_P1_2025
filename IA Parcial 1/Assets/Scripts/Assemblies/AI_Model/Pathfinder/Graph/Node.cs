namespace AI_Model.Pathfinding
{
    public class Node<Coordinate> : INode, IWeightedNode, INode<Coordinate>
    {
        private Coordinate coordinate;
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