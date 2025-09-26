using System.Collections.Generic;
using UnityEngine;

namespace RTS.Model
{
    public class Game
    {
        public readonly List<Villager> villagers = new List<Villager>();
        public readonly List<Convoy> convoys = new List<Convoy>();
        public readonly Map map;

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
        }
    }
}