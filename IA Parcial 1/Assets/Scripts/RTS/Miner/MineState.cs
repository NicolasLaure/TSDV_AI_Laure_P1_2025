// using System;
// using FSM;
// using RTS.Miner;
// using UnityEngine;
//
// public class MineState : State
// {
//     public override Type[] OnTickParametersTypes => new Type[]
//     {
//         typeof(Inventory),
//         typeof(float)
//     };
//
//     private float startTime;
//
//     public override BehaviourActions GetOnEnterBehaviours(params object[] parameters)
//     {
//         BehaviourActions behaviourActions = new BehaviourActions();
//         behaviourActions.AddMainThreadBehaviour(0, () => { startTime = Time.time; });
//         return behaviourActions;
//     }
//
//     public override BehaviourActions GetOnTickBehaviours(params object[] parameters)
//     {
//         Inventory inventory = parameters[0] as Inventory;
//         float miningSpeed = (float)parameters[1];
//
//         BehaviourActions behaviourActions = new BehaviourActions();
//         behaviourActions.AddMainThreadBehaviour(0, () =>
//         {
//             if (Time.time - startTime >= miningSpeed)
//             {
//                 startTime = Time.time;
//                 inventory.heldResources = Mathf.Clamp(inventory.heldResources + 1, 0, inventory.size);
//             }
//         });
//
//         behaviourActions.SetTransitionBehaviour(() =>
//         {
//             if (inventory.heldResources == inventory.size)
//                 OnFlag?.Invoke(MinerAgent.Flags.OnBagFull);
//         });
//
//         return behaviourActions;
//     }
// }