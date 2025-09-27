using System.Collections.Generic;
using UnityEngine;

namespace RTS.Model
{
    public class Game
    {
        public readonly List<Villager> villagers = new List<Villager>();
        public readonly List<Convoy> convoys = new List<Convoy>();
        public readonly Map map;

        private int villagerCost = 5;
        private int convoyCost = 10;

        public Game(int width, int height, int minesQty)
        {
            map = new Map(width, height, minesQty);

            villagers.Add(new Villager(map, map.grid.GetNeighbours(map.hqNode)[0]));
        }

        public void Tick(float delta)
        {
            foreach (Villager villager in villagers)
                villager.Tick(delta);

            foreach (Convoy convoy in convoys)
                convoy.Tick(delta);

            map.Tick();
        }

        public void TryBuyVillager()
        {
            if (map.headquarters.heldResources < villagerCost)
                return;

            map.headquarters.heldResources -= villagerCost;
            villagers.Add(new Villager(map, map.grid.GetNeighbours(map.hqNode)[0]));
        }

        public void TryBuyConvoy()
        {
            if (map.headquarters.heldResources < convoyCost)
                return;

            map.headquarters.heldResources -= convoyCost;
            convoys.Add(new Convoy(map, map.grid.GetNeighbours(map.hqNode)[0]));
        }
    }
}