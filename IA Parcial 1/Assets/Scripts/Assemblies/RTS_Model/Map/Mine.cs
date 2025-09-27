using UnityEditor.PackageManager;

namespace RTS.Model
{
    public class Mine : MapEntity
    {
        public int resources = 25;
        public int food = 0;
        public bool ShouldRemove => resources == 0;

        public bool TryExtract()
        {
            if (resources > 0)
            {
                resources--;
                return true;
            }

            return false;
        }

        public void DepositFood(int quantity)
        {
            food += quantity;
        }

        public bool TryGetFood()
        {
            if (food <= 0)
                return false;

            food--;
            return true;
        }
    }
}