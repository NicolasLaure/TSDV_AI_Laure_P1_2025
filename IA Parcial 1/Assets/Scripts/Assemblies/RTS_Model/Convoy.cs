using System;
using AI_Model.Pathfinding;

namespace RTS.Model
{
    public sealed class Convoy : WorkerAgent
    {
        public Convoy(Map map, MapNode startPos) : base(map, startPos)
        {
            fsm = new FSM<States, Flags>(States.WalkTowardsMine);
            AddStates();
            AddTransitions();
        }

        protected override void AddStates()
        {
            fsm.AddState<WalkState>(States.WalkTowardsBase,
                onEnterParams: () => new object[] { currentPath },
                onTickParams: () => new object[] { agentPosition });

            fsm.AddState<WalkState>(States.WalkTowardsMine,
                onEnterParams: () => new object[] { currentPath },
                onTickParams: () => new object[] { agentPosition });

            fsm.AddState<MineState>(States.Work,
                onTickParams: () => new object[] { map.headquarters, inventory, miningSpeed });
            fsm.AddState<UnloadState>(States.Unload,
                onTickParams: () => new object[]
                    { closestMineNode.heldEntity, inventory, unloadingSpeed });
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
                    closestMineNode = FindClosestMine();
                    currentPath.nodes = pathfinder.FindPath(agentPosition, closestMineNode);
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
                    currentPath.nodes = pathfinder.FindPath(agentPosition, map.hqNode);
                    /*Debug.Log("Bag Empty Returning to mine");*/
                });
        }
    }
}