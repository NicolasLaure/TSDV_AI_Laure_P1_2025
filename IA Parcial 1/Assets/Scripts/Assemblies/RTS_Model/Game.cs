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
            for (int i = 0; i < minesQty; i++)
                map.AddRandomMine();

            villagers.Add(new Villager(map));
        }
    }
}