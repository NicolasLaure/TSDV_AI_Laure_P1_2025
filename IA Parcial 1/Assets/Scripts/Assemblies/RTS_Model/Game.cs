using System.Collections.Generic;
using System.Threading.Tasks;
using AI_Model.Flocking;
using CustomMath;

namespace RTS.Model
{
    public sealed class Game
    {
        public readonly List<VillagerAgent> villagers = new List<VillagerAgent>();
        public readonly List<Convoy> convoys = new List<Convoy>();
        public readonly Map map;
        private Flocking<MapNode> flocking;

        private int villagerCost = 5;
        private int convoyCost = 10;

        public Game(int width, int height, float nodeSize, float nodeSpacing, int minesQty)
        {
            map = new Map(width, height, nodeSize, nodeSpacing, minesQty);
            map.AddVoronoiMap(typeof(VillagerAgent), VillagerAgent.typeToCost);
            map.AddVoronoiMap(typeof(Convoy), Convoy.typeToCost);

            MapNode neighbour = map.grid.GetNeighbours(map.hqNode)[0];
            Vec3 initialPos = map.grid.ToEntityGridAligned(neighbour.GetCoordinate());
            villagers.Add(new VillagerAgent(map, neighbour, initialPos));

            neighbour = map.grid.GetNeighbours(map.hqNode)[1];
            initialPos = map.grid.ToEntityGridAligned(neighbour.GetCoordinate());
            villagers.Add(new VillagerAgent(map, neighbour, initialPos));

            neighbour = map.grid.GetNeighbours(map.hqNode)[2];
            initialPos = map.grid.ToEntityGridAligned(neighbour.GetCoordinate());
            villagers.Add(new VillagerAgent(map, neighbour, initialPos));

            convoys.Add(new Convoy(map, map.grid.GetNeighbours(map.hqNode)[1]));
            List<IFlockeable<MapNode>> flockingAgents = new List<IFlockeable<MapNode>>();
            foreach (VillagerAgent villager in villagers)
                flockingAgents.Add(villager);

            flocking = new Flocking<MapNode>(flockingAgents, map.grid);
        }

        public void Tick(float delta)
        {
            ParallelOptions parallelOptions = new ParallelOptions();
            parallelOptions.MaxDegreeOfParallelism = 32;
            Parallel.ForEach(villagers, parallelOptions, villager => { villager.Tick(delta); });
            Parallel.ForEach(villagers, parallelOptions,
                villager => { villager.Flock(flocking.GetBoidsInsideRadius(villager), delta); });
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
            spawnedVillager =
                new VillagerAgent(map, map.hqNode, map.grid.ToEntityGridAligned(map.hqNode.GetCoordinate()));
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