using System.Collections.Generic;
using AI_Model.Pathfinding;

namespace RTS.Model
{
    public abstract class TravelerAgent
    {
        public enum States
        {
            WalkTowardsMine,
            WalkTowardsBase,
            Work,
            Unload
        }

        public enum Flags
        {
            OnTargetReach,
            OnBagFull,
            OnBagEmpty
        }

        protected FSM<States, Flags> fsm;

        public MapNode closestMineNode;

        public MapNode agentPosition;

        public readonly float speed;
        public readonly float miningSpeed;

        public readonly float unloadingSpeed;

        public readonly Inventory inventory;
        public readonly float proximityThreshold;

        protected float delta;

        protected Map map;

        protected AStarPathfinder<MapNode> pathfinder;
        protected List<MapNode> currentPath = new List<MapNode>();

        public TravelerAgent(Map map, MapNode startPos)
        {
            this.map = map;
            pathfinder = new AStarPathfinder<MapNode>(map.grid);
            agentPosition = startPos;
            closestMineNode = FindClosestMine();
            inventory = new Inventory();
        }

        public void Tick(float delta)
        {
            fsm.Tick();
        }

        public MapNode FindClosestMine()
        {
            return map.voronoi.GetClosestLandMark(agentPosition);
        }

        protected abstract void AddStates();
        protected abstract void AddTransitions();
    }
}