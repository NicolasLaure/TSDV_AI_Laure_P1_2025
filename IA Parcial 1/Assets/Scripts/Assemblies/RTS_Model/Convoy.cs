namespace RTS.Model
{
    public sealed class Convoy : TravelerAgent
    {
        public Convoy(Map map) : base(map)
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

            fsm.AddState<MineState>(States.Work,
                onTickParams: () => new object[] { inventory, miningSpeed, map.headquarters });
            fsm.AddState<UnloadState>(States.Unload,
                onTickParams: () => new object[] { inventory, unloadingSpeed, closestMineNode });
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
                    FindClosestMine();
                    currentPath = pathfinder.FindPath(agentPosition, closestMineNode);
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
                    currentPath = pathfinder.FindPath(agentPosition, map.hqNode);
                    /*Debug.Log("Bag Empty Returning to mine");*/
                });
        }
    }
}