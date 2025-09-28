using System;
using System.Collections.Generic;
using AI_Model.Pathfinding;

namespace RTS.Model
{
    public sealed class Convoy : WorkerAgent
    {
        private int inventorySize = 10;

        public Convoy(Map map, MapNode startPos) : base(map, startPos)
        {
            inventory.size = inventorySize;
            fsm = new FSM<States, Flags>(States.Idle);
            AddStates();
            AddTransitions();
            fsm.Init();
        }

        protected override void AddStates()
        {
            Func<WorkerAgent> agentFunc = () => this;
            
            fsm.AddState<IdleState>(States.Idle,
            onTickParams: () => new object[] { map, agentFunc });

            fsm.AddState<WalkState>(States.WalkTowardsBase,
            onEnterParams: () => new object[] { map.hqNode, currentPath },
            onTickParams: () => new object[] { agentFunc });

            fsm.AddState<WalkState>(States.WalkTowardsMine,
            onEnterParams: () => new object[] { closestMineNode, currentPath },
            onTickParams: () => new object[] { agentFunc });

            fsm.AddState<GatherFoodState>(States.Work, onEnterParams: () => new object[] { map.headquarters },
            onTickParams: () => new object[] { inventory, miningSpeed });
            
            fsm.AddState<UnloadState>(States.Unload,
            onTickParams: () => new object[]
                { closestMineNode.heldEntity, inventory, unloadingSpeed });
        }

        protected override void AddTransitions()
        {
            fsm.SetTransition(States.Idle, Flags.OnMineEmpty, States.WalkTowardsMine,
            () => { currentPath.nodes = pathfinder.FindPath(agentPosition, closestMineNode); });

            fsm.SetTransition(States.WalkTowardsMine, Flags.OnTargetReach, States.Unload,
            () =>
            {
                /*Debug.Log("MineReached");*/
            });

            fsm.SetTransition(States.WalkTowardsMine, Flags.OnMineEmpty, States.Idle,
            () =>
            {
                /*Debug.Log("MineReached");*/
            });

            fsm.SetTransition(States.Work, Flags.OnBagFull, States.Idle,
            () => { });

            fsm.SetTransition(States.WalkTowardsBase, Flags.OnTargetReach, States.Work,
            () =>
            {
                /*Debug.Log("BaseReached");*/
            });

            fsm.SetTransition(States.Unload, Flags.OnBagEmpty, States.WalkTowardsBase,
            () =>
            {
                currentPath.nodes = pathfinder.FindPath(agentPosition, map.hqNode);
                /*Debug.Log("Bag Empty Returning to mine");*/
            });
        }
    }
}