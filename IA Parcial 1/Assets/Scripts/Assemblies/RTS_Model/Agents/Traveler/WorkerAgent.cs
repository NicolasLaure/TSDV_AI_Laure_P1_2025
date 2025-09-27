using System.Collections.Generic;
using AI_Model.Pathfinding;

namespace RTS.Model
{
    public abstract class WorkerAgent
    {
        public enum States
        {
            WalkTowardsMine,
            WalkTowardsBase,
            Work,
            Unload,
            Wait
        }

        public enum Flags
        {
            OnTargetReach,
            OnBagFull,
            OnBagEmpty,
            OnMineEmpty,
            OnHungry
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
        public Path<MapNode> currentPath = new Path<MapNode>();
        public List<MapNode> CurrentPath => currentPath.nodes;

        public WorkerAgent(Map map, MapNode startPos)
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