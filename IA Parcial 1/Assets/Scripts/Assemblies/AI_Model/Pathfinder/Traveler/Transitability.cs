namespace AI_Model.Pathfinding
{
    public class Transitability
    {
        public int weight;
        public bool isWalkable ;

        public Transitability(int weight, bool isWalkable)
        {
            this.weight = weight;
            this.isWalkable = isWalkable;
        }
    }
}
