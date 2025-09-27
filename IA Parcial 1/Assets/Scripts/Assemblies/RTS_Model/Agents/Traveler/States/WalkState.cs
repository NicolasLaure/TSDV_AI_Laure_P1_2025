using System;
using AI_Model.Pathfinding;
using FSM;

namespace RTS.Model
{
    public class WalkState : State
    {
        public override Type[] OnEnterParametersTypes => new Type[]
        {
            typeof(Path<MapNode>)
        };

        public override Type[] OnTickParametersTypes => new Type[]
        {
            typeof(Func<WorkerAgent>),
        };

        private Path<MapNode> path;
        private int step;

        public override BehaviourActions GetOnEnterBehaviours(params object[] parameters)
        {
            BehaviourActions behaviourActions = new BehaviourActions();
            behaviourActions.AddMainThreadBehaviour(0, () =>
            {
                path = parameters[0] as Path<MapNode>;
                step = 0;
            });

            behaviourActions.SetTransitionBehaviour(() =>
            {
                // if (Vector3.Distance(agentPosition.position, targetTransform.position) < proximityThreshold)
                //     OnFlag?.Invoke(TravelerAgent.Flags.OnTargetReach);
            });

            return behaviourActions;
        }

        public override BehaviourActions GetOnTickBehaviours(params object[] parameters)
        {
            WorkerAgent workerAgent = (parameters[0] as Func<WorkerAgent>)?.Invoke();

            BehaviourActions behaviourActions = new BehaviourActions();
            behaviourActions.AddMainThreadBehaviour(0,
            () =>
            {
                if (step >= path.nodes.Count) return;

                workerAgent.agentPosition = workerAgent.CurrentPath[step];
                step++;
            });

            behaviourActions.SetTransitionBehaviour(() =>
            {
                if (workerAgent.agentPosition == workerAgent.CurrentPath[^1])
                    OnFlag?.Invoke(WorkerAgent.Flags.OnTargetReach);
            });

            return behaviourActions;
        }
    }
}