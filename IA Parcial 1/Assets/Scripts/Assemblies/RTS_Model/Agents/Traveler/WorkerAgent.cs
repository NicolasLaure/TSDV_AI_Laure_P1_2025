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
            SeekShelter,
            Hide,
            Work,
            Unload,
            Idle
        }

        public enum Flags
        {
            OnTargetReach,
            OnBagFull,
            OnBagEmpty,
            OnMineEmpty,
            OnAlert
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
            this.delta = delta;
            fsm.Tick();
        }

        public MapNode FindClosestMine()
        {
            return map.voronoi.GetClosestLandMark(agentPosition);
        }

        protected abstract void AddStates();
        protected abstract void AddTransitions();

        public void Alert()
        {
            fsm.Transition(Flags.OnAlert);
        }

        protected void GetMinePath()
        {
            closestMineNode = FindClosestMine();
            currentPath.nodes = pathfinder.FindPath(agentPosition, closestMineNode);
            /*Debug.Log("MineReached");*/
        }

        protected void GetHqPath()
        {
            currentPath.nodes = pathfinder.FindPath(agentPosition, map.hqNode);
        }
    }
}