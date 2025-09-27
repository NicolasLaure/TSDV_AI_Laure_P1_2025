using System;
using System.IO;
using AI_Model.Pathfinding;

namespace RTS.Model
{
    public sealed class Villager : TravelerAgent
    {
        private float sleepDuration = 0.1f;

        private int maxFood = 3;
        private int currentFood;

        public Villager(Map map, MapNode startPos) : base(map, startPos)
        {
            fsm = new FSM<States, Flags>(States.WalkTowardsMine);
            AddStates();
            AddTransitions();
            closestMineNode = FindClosestMine();
            currentPath.nodes = pathfinder.FindPath(agentPosition, closestMineNode);
            fsm.Init();
            currentFood = maxFood;
        }

        protected override void AddStates()
        {
            Func<TravelerAgent> agentFunc = () => this;

            fsm.AddState<WalkState>(States.WalkTowardsBase,
            onEnterParams: () => new object[] { currentPath },
            onTickParams: () => new object[] { agentFunc });

            fsm.AddState<WalkState>(States.WalkTowardsMine,
            onEnterParams: () => new object[] { currentPath },
            onTickParams: () => new object[] { agentFunc });

            fsm.AddState<MineState>(States.Work,
            onTickParams: () => new object[]
                { closestMineNode.heldEntity, inventory, miningSpeed });
            fsm.AddState<UnloadState>(States.Unload, onTickParams: () => new object[]
            {
                map.headquarters, inventory, unloadingSpeed
            });
        }

        protected override void AddTransitions()
        {
            fsm.SetTransition(States.WalkTowardsMine, Flags.OnTargetReach, States.Work,
            () =>
            {
                /*Debug.Log("MineReached");*/
            });
            fsm.SetTransition(States.Work, Flags.OnBagFull, States.WalkTowardsBase,
            () =>
            {
                currentPath.nodes = pathfinder.FindPath(agentPosition, map.hqNode);
                /*Debug.Log("BagFull Returning To Base");*/
            });
            fsm.SetTransition(States.WalkTowardsBase, Flags.OnTargetReach, States.Unload,
            () =>
            {
                /*Debug.Log("BaseReached");*/
            });
            fsm.SetTransition(States.Unload, Flags.OnBagEmpty, States.WalkTowardsMine,
            () =>
            {
                closestMineNode = FindClosestMine();
                currentPath.nodes = pathfinder.FindPath(agentPosition, closestMineNode);
                /*Debug.Log("Bag Empty Returning to mine");*/
            });
        }
    }
}