using System;
using System.Collections.Generic;
using AI_Model.Pathfinding;
using AI_Model.Utilities;
using AI_Model.Voronoi;

namespace RTS.Model
{
    public class Map
    {
        public HeadQuarters headquarters = new HeadQuarters();
        public MapNode hqNode;
        public Grid<MapNode> grid;

        public Voronoi<MapNode> voronoi;
        private Random rnGen = new Random();

        private Dictionary<Mine, MapNode> mineToNode = new Dictionary<Mine, MapNode>();

        public Map(int width, int height, int minesQty)
        {
            grid = new Grid<MapNode>(width, height);
            int hqPos = rnGen.Next(0, grid.nodes.Count);
            grid.nodes[hqPos].heldEntity = headquarters;
            hqNode = grid.nodes[hqPos];
            
            voronoi = new Voronoi<MapNode>(grid);
            PopulateMines(minesQty);
        }

        public void PopulateMines(int minesQty)
        {
            for (int i = 0; i < minesQty; i++)
                AddRandomMine();

            voronoi.Bake(mineToNode.Values);
        }

        public void AddRandomMine()
        {
            Mine newMine = new Mine();
            MapNode mineNode = null;
            do
            {
                int num = rnGen.Next(0, grid.nodes.Count);
                if (grid.nodes[num].heldEntity == null)
                    mineNode = grid.nodes[num];
            } while (mineNode == null);

            mineNode.heldEntity = newMine;
            mineToNode.Add(newMine, mineNode);
        }

        public void RemoveMine(Mine mine)
        {
            mineToNode.Remove(mine);
            voronoi.Bake(mineToNode.Values);
        }
    }
}