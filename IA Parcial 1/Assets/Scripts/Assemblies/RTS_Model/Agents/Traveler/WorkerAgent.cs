using System;
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

        protected Type agentType;

        public WorkerAgent(Map map, MapNode startPos, Type agentType, Dictionary<Enum, Transitability> typeCost)
        {
            this.map = map;
            this.agentType = agentType;
            pathfinder = new AStarPathfinder<MapNode>(map.grid, typeCost);
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
            if (!map.agentTypeToVoronoi.ContainsKey(agentType))
                throw new Exception("AgentType Not Found");

            return map.agentTypeToVoronoi[agentType].GetClosestLandMark(agentPosition);
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
            if (closestMineNode == null)
                throw new Exception("ClosestMine not found");
            
            currentPath.nodes = pathfinder.FindPath(agentPosition, closestMineNode);
            /*Debug.Log("MineReached");*/
        }

        protected void GetHqPath()
        {
            currentPath.nodes = pathfinder.FindPath(agentPosition, map.hqNode);
        }
    }
}