// using System;
// using FSM;
// using UnityEngine;
//
// namespace RTS.Miner
// {
//     public class UnloadState : State
//     {
//         public override Type[] OnTickParametersTypes => new Type[]
//         {
//             typeof(Inventory),
//             typeof(float),
//             typeof(HeadQuartersModel)
//         };
//
//         private float startTime;
//
//         public override BehaviourActions GetOnEnterBehaviours(params object[] parameters)
//         {
//             BehaviourActions behaviourActions = new BehaviourActions();
//             behaviourActions.AddMainThreadBehaviour(0, () => { startTime = Time.time; });
//             return behaviourActions;
//         }
//
//         public override BehaviourActions GetOnTickBehaviours(params object[] parameters)
//         {
//             Inventory inventory = parameters[0] as Inventory;
//             float unloadSpeed = (float)parameters[1];
//             HeadQuartersModel hq = parameters[2] as HeadQuartersModel;
//
//             BehaviourActions behaviourActions = new BehaviourActions();
//             behaviourActions.AddMainThreadBehaviour(0, () =>
//             {
//                 if (Time.time - startTime >= unloadSpeed)
//                 {
//                     startTime = Time.time;
//                     inventory.heldResources = Mathf.Clamp(inventory.heldResources - 1, 0, inventory.size);
//                     hq.heldResources++;
//                 }
//             });
//
//             behaviourActions.SetTransitionBehaviour(() =>
//             {
//                 if (inventory.heldResources == 0)
//                     OnFlag?.Invoke(MinerAgent.Flags.OnBagEmpty);
//             });
//
//             return behaviourActions;
//         }
//     }
// }