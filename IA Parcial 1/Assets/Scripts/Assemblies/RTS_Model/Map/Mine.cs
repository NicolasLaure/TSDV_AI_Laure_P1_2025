namespace RTS.Model
{
    public class Mine : MapEntity
    {
        public int resources = 25;
        public int food = 0;
        public bool ShouldRemove => resources <= 0;
        public int workingVillagers = 0;
        public int suppliers = 0;

        public bool CanExtract()
        {
            return resources > 0;
        }

        public void Extract()
        {
            resources--;
        }

        public int TryGetFood(int maxFood)
        {
            if (food >= maxFood)
            {
                food -= maxFood;
                return maxFood;
            }

            int aux = food;
            food = 0;
            return aux;
        }

        public override void AddResources(int qty)
        {
            food += qty;
        }
    }
}