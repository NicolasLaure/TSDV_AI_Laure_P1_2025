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
            typeof(Func<MapNode>),
        };

        private Path<MapNode> path;
        private int step = 0;

        public override BehaviourActions GetOnEnterBehaviours(params object[] parameters)
        {
            path = parameters[0] as Path<MapNode>;
            step = 0;
            BehaviourActions behaviourActions = new BehaviourActions();
            behaviourActions.AddMainThreadBehaviour(0, () => { });

            behaviourActions.SetTransitionBehaviour(() =>
            {
                // if (Vector3.Distance(agentPosition.position, targetTransform.position) < proximityThreshold)
                //     OnFlag?.Invoke(TravelerAgent.Flags.OnTargetReach);
            });

            return behaviourActions;
        }

        public override BehaviourActions GetOnTickBehaviours(params object[] parameters)
        {
            MapNode agentPosition = (parameters[0] as Func<MapNode>)?.Invoke();

            BehaviourActions behaviourActions = new BehaviourActions();
            behaviourActions.AddMainThreadBehaviour(0,
                () =>
                {
                    agentPosition.SetCoordinate(path.nodes[step].GetCoordinate());
                    step++;
                });

            behaviourActions.SetTransitionBehaviour(() =>
            {
                // if (Vector3.Distance(agentPosition.position, targetTransform.position) < proximityThreshold)
                //     OnFlag?.Invoke(TravelerAgent.Flags.OnTargetReach);
            });

            return behaviourActions;
        }
    }
}