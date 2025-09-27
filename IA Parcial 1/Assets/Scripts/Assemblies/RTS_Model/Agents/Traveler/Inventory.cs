using System;

namespace RTS.Model
{
    [Serializable]
    public class Inventory
    {
        public int heldResources = 0;
        public int size = 15;
        public int maxFood = 3;
        public int food;

        public Inventory()
        {
            food = maxFood;
        }
    }
}