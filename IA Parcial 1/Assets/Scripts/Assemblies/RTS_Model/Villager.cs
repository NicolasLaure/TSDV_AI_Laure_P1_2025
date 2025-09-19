using System.Collections.Generic;
using AI_Model.Pathfinding;

namespace RTS.Model
{
    public sealed class Villager : TravelerAgent
    {
        private float sleepDuration = 0.1f;

        public Villager(Map map) : base(map)
        {
            fsm = new FSM<States, Flags>(States.WalkTowardsMine);
            AddStates();
            AddTransitions();
        }

        protected override void AddStates()
        {
            fsm.AddState<WalkState>(States.WalkTowardsBase,
                onEnterParams: () => new object[] { currentPath },
                onTickParams: () => new object[] { agentPosition, speed, proximityThreshold, delta });

            fsm.AddState<WalkState>(States.WalkTowardsMine,
                onEnterParams: () => new object[] { currentPath },
                onTickParams: () => new object[]
                    { agentPosition, speed, proximityThreshold, delta });

            fsm.AddState<MineState>(States.Work, onTickParams: () => new object[] { inventory, miningSpeed });
            fsm.AddState<UnloadState>(States.Unload,
                onTickParams: () => new object[] { inventory, unloadingSpeed, map.headquarters });
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
                    currentPath = pathfinder.FindPath(agentPosition, map.hqNode);
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
                    FindClosestMine();
                    currentPath = pathfinder.FindPath(agentPosition, closestMineNode);
                    /*Debug.Log("Bag Empty Returning to mine");*/
                });
        }
    }
}