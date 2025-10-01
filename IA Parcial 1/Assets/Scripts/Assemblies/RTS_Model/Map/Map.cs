using System;
using System.Collections.Generic;
using AI_Model.Pathfinding;
using AI_Model.Voronoi;

namespace RTS.Model
{
    public class Map
    {
        public HeadQuarters headquarters = new HeadQuarters();
        public MapNode hqNode;
        public Grid<MapNode> grid;

        public Dictionary<Type, Voronoi<MapNode>> agentTypeToVoronoi = new Dictionary<Type, Voronoi<MapNode>>();
        private Random rnGen = new Random();

        private Dictionary<Mine, MapNode> mineToNode = new Dictionary<Mine, MapNode>();

        public Action<MapNode> onMineRemove;
        public Action onBake;

        public Map(int width, int height, int minesQty)
        {
            grid = new Grid<MapNode>(width, height);
            int hqPos = rnGen.Next(0, grid.nodes.Count);
            //int hqPos = 14;
            grid.nodes[hqPos].heldEntity = headquarters;
            hqNode = grid.nodes[hqPos];
            foreach (MapNode node in grid.nodes)
            {
                int randomTile = rnGen.Next(0, (int)TileType.Water + 1);
                SetTileType(node, (TileType)randomTile);
            }

            hqNode.SetTileType((int)TileType.Hill);

            PopulateMines(minesQty);
        }

        public void Tick()
        {
            ClearEmptyMines();
        }

        public void PopulateMines(int minesQty)
        {
            for (int i = 0; i < minesQty; i++)
                AddRandomMine();

            Bake();
        }

        public void AddRandomMine()
        {
            Mine newMine = new Mine();
            MapNode mineNode = null;
            do
            {
                int num = rnGen.Next(0, grid.nodes.Count);
                if (grid.nodes[num].heldEntity == null)
                {
                    bool isClearAround = true;
                    foreach (MapNode nodes in grid.GetNeighbours(grid.nodes[num]))
                    {
                        if (nodes.heldEntity != null)
                            isClearAround = false;
                    }

                    if (isClearAround)
                        mineNode = grid.nodes[num];
                }
            } while (mineNode == null);

            mineNode.heldEntity = newMine;
            mineToNode.Add(newMine, mineNode);
        }

        private void ClearEmptyMines()
        {
            List<Mine> minesToRemove = new List<Mine>();
            foreach (Mine mine in mineToNode.Keys)
            {
                if (mine.ShouldRemove)
                    minesToRemove.Add(mine);
            }

            if (minesToRemove.Count == 0)
                return;

            foreach (Mine mine in minesToRemove)
            {
                onMineRemove?.Invoke(mineToNode[mine]);
                mineToNode.Remove(mine);
            }

            Bake();
        }

        public void RemoveMine(Mine mine)
        {
            onMineRemove?.Invoke(mineToNode[mine]);
            mineToNode.Remove(mine);
            Bake();
        }

        public void SetTileType(MapNode node, TileType type)
        {
            if (node == hqNode)
                return;

            node.SetTileType((int)type);
        }

        public void AddVoronoiMap(Type agentType, Dictionary<int, Transitability> typeToWeight)
        {
            agentTypeToVoronoi.Add(agentType, new Voronoi<MapNode>(grid));
            Bake();
        }

        private void Bake()
        {
            foreach (Voronoi<MapNode> voronoi in agentTypeToVoronoi.Values)
            {
                voronoi.Bake(mineToNode.Values);
            }

            onBake?.Invoke();
        }

        public List<MapNode> GetMineLocations()
        {
            List<MapNode> mineLocations = new List<MapNode>();
            foreach (MapNode mapNode in mineToNode.Values)
            {
                mineLocations.Add(mapNode);
            }

            return mineLocations;
        }

        public List<Mine> GetMines()
        {
            List<Mine> mines = new List<Mine>();
            foreach (Mine mine in mineToNode.Keys)
            {
                mines.Add(mine);
            }

            return mines;
        }

        public MapNode GetNode(Mine entity)
        {
            if (mineToNode.ContainsKey(entity))
                return mineToNode[entity];

            return null;
        }
    }
}