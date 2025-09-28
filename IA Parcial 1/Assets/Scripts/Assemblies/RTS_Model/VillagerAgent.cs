using System;

namespace RTS.Model
{
    public sealed class VillagerAgent : WorkerAgent
    {
        private float sleepDuration = 0.1f;

        private int maxFood = 3;

        public int MaxFood => maxFood;
        public int CurrentFood => inventory.food;

        public Action onFoodUpdate;

        public VillagerAgent(Map map, MapNode startPos) : base(map, startPos)
        {
            fsm = new FSM<States, Flags>(States.WalkTowardsMine);
            AddStates();
            AddTransitions();
            closestMineNode = FindClosestMine();
            currentPath.nodes = pathfinder.FindPath(agentPosition, closestMineNode);
            fsm.Init();
        }

        protected override void AddStates()
        {
            Func<WorkerAgent> agentFunc = () => this;

            fsm.AddState<WalkState>(States.WalkTowardsBase,
            onEnterParams: () => new object[] { map.hqNode, currentPath },
            onTickParams: () => new object[] { agentFunc });

            fsm.AddState<WalkState>(States.WalkTowardsMine,
            onEnterParams: () => new object[] { closestMineNode, currentPath },
            onTickParams: () => new object[] { agentFunc });

            Func<Action> onFoodFunc = () => onFoodUpdate;
            fsm.AddState<MineState>(States.Work, onEnterParams: () => new object[] { closestMineNode.heldEntity },
            onTickParams: () => new object[]
                { inventory, miningSpeed, onFoodFunc });

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
            fsm.SetTransition(States.WalkTowardsMine, Flags.OnMineEmpty, States.WalkTowardsMine,
            () =>
            {
                closestMineNode = FindClosestMine();
                currentPath.nodes = pathfinder.FindPath(agentPosition, closestMineNode);
                /*Debug.Log("MineReached");*/
            });

            fsm.SetTransition(States.Work, Flags.OnBagFull, States.WalkTowardsBase,
            () =>
            {
                currentPath.nodes = pathfinder.FindPath(agentPosition, map.hqNode);
                /*Debug.Log("BagFull Returning To Base");*/
            });
            fsm.SetTransition(States.Work, Flags.OnMineEmpty, States.WalkTowardsMine,
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
                closestMineNode = FindClosestMine();
                currentPath.nodes = pathfinder.FindPath(agentPosition, closestMineNode);
                /*Debug.Log("Bag Empty Returning to mine");*/
            });
        }
    }
}