namespace RTS.Model
{
    public class HeadQuarters : MapEntity
    {
        public int heldResources;
        private int foodDrop = 10;

        public int GetFood()
        {
            return foodDrop;
        }

        public override void AddResources(int qty)
        {
            heldResources += qty;
        }
    }
}