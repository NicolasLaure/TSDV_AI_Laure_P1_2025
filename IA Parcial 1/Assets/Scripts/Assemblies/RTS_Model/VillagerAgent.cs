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

            fsm.AddState<WalkState>(States.SeekShelter,
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

            fsm.AddState<HideState>(States.Hide,
            onEnterParams: () => new object[] { map.headquarters, inventory, true });
        }

        protected override void AddTransitions()
        {
            fsm.SetTransition(States.WalkTowardsMine, Flags.OnTargetReach, States.Work,
            () =>
            {
                /*Debug.Log("MineReached");*/
            });
            fsm.SetTransition(States.WalkTowardsMine, Flags.OnMineEmpty, States.WalkTowardsMine, GetMinePath);

            fsm.SetTransition(States.Work, Flags.OnBagFull, States.WalkTowardsBase, GetHqPath);

            fsm.SetTransition(States.Work, Flags.OnMineEmpty, States.WalkTowardsMine, GetMinePath);

            fsm.SetTransition(States.WalkTowardsBase, Flags.OnTargetReach, States.Unload, () =>
            {
                /*Debug.Log("BaseReached");*/
            });

            fsm.SetTransition(States.Unload, Flags.OnBagEmpty, States.WalkTowardsMine, GetMinePath);

            //Alert
            fsm.SetTransition(States.Work, Flags.OnAlert, States.SeekShelter, GetHqPath);
            fsm.SetTransition(States.WalkTowardsBase, Flags.OnAlert, States.SeekShelter, GetHqPath);
            fsm.SetTransition(States.WalkTowardsMine, Flags.OnAlert, States.SeekShelter, GetHqPath);
            fsm.SetTransition(States.Idle, Flags.OnAlert, States.SeekShelter, () => { });
            fsm.SetTransition(States.Unload, Flags.OnAlert, States.SeekShelter, () => { });

            fsm.SetTransition(States.SeekShelter, Flags.OnTargetReach, States.Hide, () => { });

            fsm.SetTransition(States.SeekShelter, Flags.OnAlert, States.WalkTowardsMine, () =>
            {
                currentPath.nodes = pathfinder.FindPath(agentPosition, closestMineNode);
            });
            fsm.SetTransition(States.Hide, Flags.OnAlert, States.WalkTowardsMine, () =>
            {
                currentPath.nodes = pathfinder.FindPath(agentPosition, closestMineNode);
            });
        }
    }
}