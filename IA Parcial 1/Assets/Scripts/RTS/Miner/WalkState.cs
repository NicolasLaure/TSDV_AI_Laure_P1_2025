// using System;
// using FSM;
// using RTS.Miner;
// using UnityEngine;
//
// public class WalkState : State
// {
//     public override Type[] OnTickParametersTypes => new Type[]
//     {
//         typeof(Transform),
//         typeof(Transform),
//         typeof(float),
//         typeof(float),
//         typeof(float)
//     };
//
//     public override BehaviourActions GetOnTickBehaviours(params object[] parameters)
//     {
//         Transform agentTransform = parameters[0] as Transform;
//         Transform targetTransform = parameters[1] as Transform;
//         float speed = (float)parameters[2];
//         float proximityThreshold = (float)parameters[3];
//         float delta = (float)parameters[4];
//
//         BehaviourActions behaviourActions = new BehaviourActions();
//         behaviourActions.AddMainThreadBehaviour(0, () => { agentTransform.position += (targetTransform.position - agentTransform.position).normalized * (speed * delta); });
//
//         behaviourActions.SetTransitionBehaviour(() =>
//         {
//             if (Vector3.Distance(agentTransform.position, targetTransform.position) < proximityThreshold)
//                 OnFlag?.Invoke(MinerAgent.Flags.OnTargetReach);
//         });
//
//         return behaviourActions;
//     }
// }