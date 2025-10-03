using System;
using System.Collections.Generic;
using AI_Model.Flocking;
using AI_Model.Pathfinding;
using AI_Model.Utilities;
using CustomMath;

namespace RTS.Model
{
    public sealed class VillagerAgent : WorkerAgent, IFlockeable<MapNode>
    {
        private float sleepDuration = 0.1f;

        public float turnSpeed = 5f;
        public float detectionRadious = 3.0f;

        private float alignmentFactor = 1f;
        private float cohesionFactor = 0.5f;
        private float separationFactor = 1.5f;
        private float directionFactor = 1f;

        private int maxFood = 3;

        private Func<VillagerAgent, Vec3> Alignment;
        private Func<VillagerAgent, Vec3> Cohesion;
        private Func<VillagerAgent, Vec3> Separation;
        private Func<VillagerAgent, Vec3> Direction;

        public int MaxFood => maxFood;
        public int CurrentFood => inventory.food;

        public Action onFoodUpdate;

        public static readonly Dictionary<int, Transitability> typeToCost = new Dictionary<int, Transitability>()
        {
            { (int)TileType.Hill, new Transitability(1, true) },
            { (int)TileType.Mountain, new Transitability(3, true) },
            { (int)TileType.Water, new Transitability(1, false) }
        };

        public VillagerAgent(Map map, MapNode startPos, Vec3 initialPosition) : base(map, startPos,
            typeof(VillagerAgent), typeToCost)
        {
            fsm = new FSM<States, Flags>(States.WalkTowardsMine);
            transform.Position = initialPosition;
            AddStates();
            AddTransitions();
            GetMinePath();
            fsm.Init();
        }

        public void Flock(List<IFlockeable<MapNode>> closeAgents, float delta)
        {
            transform.Position += transform.forward * speed * delta;
            transform.forward = Vec3.Lerp(transform.forward, ACS(), turnSpeed * delta);
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
                    { inventory, miningSpeed, onFoodFunc, delta });

            fsm.AddState<UnloadState>(States.Unload, onTickParams: () => new object[]
            {
                map.headquarters, inventory, unloadingSpeed, delta
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
            fsm.SetTransition(States.Idle, Flags.OnAlert, States.SeekShelter, GetHqPath);
            fsm.SetTransition(States.Unload, Flags.OnAlert, States.SeekShelter, GetHqPath);

            fsm.SetTransition(States.SeekShelter, Flags.OnTargetReach, States.Hide, () => { });

            fsm.SetTransition(States.SeekShelter, Flags.OnAlert, States.WalkTowardsMine,
                () => { currentPath.nodes = pathfinder.FindPath(agentPosition, closestMineNode); });
            fsm.SetTransition(States.Hide, Flags.OnAlert, States.WalkTowardsMine,
                () => { currentPath.nodes = pathfinder.FindPath(agentPosition, closestMineNode); });
        }

        public void Init(Func<IFlockeable<MapNode>, Vec3> Alignment, Func<IFlockeable<MapNode>, Vec3> Cohesion,
            Func<IFlockeable<MapNode>, Vec3> Separation, Func<IFlockeable<MapNode>, Vec3> Direction)
        {
            this.Alignment = Alignment;
            this.Cohesion = Cohesion;
            this.Separation = Separation;
            this.Direction = Direction;
        }

        public Vec3 ACS()
        {
            Vec3 ACS = (Alignment(this) * alignmentFactor) +
                       (Cohesion(this) * cohesionFactor) +
                       (Separation(this) * separationFactor) +
                       (Direction(this) * directionFactor);

            return ACS.normalizedVec3;
        }

        public Vec3 GetCoordinate()
        {
            Vec2Int pos = agentPosition.GetCoordinate();
            return new Vec3(pos.X, pos.Y, 0);
        }

        public List<MapNode> GetPath()
        {
            return currentPath.nodes;
        }

        public MyTransform GetTransform()
        {
            return transform;
        }

        public MapNode GetNextPosition()
        {
            return currentPath.NextNode();
        }

        public float GetDetectionRadius()
        {
            return detectionRadious;
        }
    }
}