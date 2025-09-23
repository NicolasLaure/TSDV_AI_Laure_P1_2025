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
            typeof(MapNode),
            typeof(float),
            typeof(float),
            typeof(float)
        };

        public override BehaviourActions GetOnTickBehaviours(params object[] parameters)
        {
            MapNode agentPosition = parameters[0] as MapNode;
            MapNode targetTransform = parameters[1] as MapNode;
            float speed = (float)parameters[2];
            float proximityThreshold = (float)parameters[3];
            float delta = (float)parameters[4];

            BehaviourActions behaviourActions = new BehaviourActions();
            behaviourActions.AddMainThreadBehaviour(0,
            () =>
            {
                // agentPosition.position += (targetTransform.position - agentPosition.position).normalized *
                //                            (speed * delta);
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