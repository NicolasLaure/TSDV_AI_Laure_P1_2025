using System.Collections.Generic;
using System.Threading.Tasks;

namespace RTS.Model
{
    public class Game
    {
        public readonly List<VillagerAgent> villagers = new List<VillagerAgent>();
        public readonly List<Convoy> convoys = new List<Convoy>();
        public readonly Map map;

        private int villagerCost = 5;
        private int convoyCost = 10;

        public Game(int width, int height, int minesQty)
        {
            map = new Map(width, height, minesQty);

            map.AddVoronoiMap(typeof(VillagerAgent), VillagerAgent.typeToCost);
            map.AddVoronoiMap(typeof(Convoy), Convoy.typeToCost);

            villagers.Add(new VillagerAgent(map, map.grid.GetNeighbours(map.hqNode)[0]));
            convoys.Add(new Convoy(map, map.grid.GetNeighbours(map.hqNode)[1]));
        }

        public void Tick(float delta)
        {
            ParallelOptions parallelOptions = new ParallelOptions();
            parallelOptions.MaxDegreeOfParallelism = 32;

            Parallel.ForEach(villagers, parallelOptions, villager => { villager.Tick(delta); });
            Parallel.ForEach(convoys, parallelOptions, convoy => { convoy.Tick(delta); });

            map.Tick();
        }

        public bool TryBuyVillager(out VillagerAgent spawnedVillager)
        {
            if (map.headquarters.heldResources < villagerCost)
            {
                spawnedVillager = null;
                return false;
            }

            map.headquarters.heldResources -= villagerCost;
            spawnedVillager = new VillagerAgent(map, map.grid.GetNeighbours(map.hqNode)[0]);
            villagers.Add(spawnedVillager);
            return true;
        }

        public bool TryBuyConvoy(out Convoy spawnedConvoy)
        {
            if (map.headquarters.heldResources < convoyCost)
            {
                spawnedConvoy = null;
                return false;
            }

            map.headquarters.heldResources -= convoyCost;
            spawnedConvoy = new Convoy(map, map.grid.GetNeighbours(map.hqNode)[0]);
            convoys.Add(spawnedConvoy);
            return true;
        }

        public void Alert()
        {
            foreach (VillagerAgent villager in villagers)
            {
                villager.Alert();
            }

            foreach (Convoy convoy in convoys)
            {
                convoy.Alert();
            }
        }
    }
}