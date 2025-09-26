// using System;
// using UnityEngine;
// using UnityEngine.Serialization;
//
// namespace RTS.Miner
// {
//     public class MinerAgent : MonoBehaviour
//     {
//         public enum States
//         {
//             WalkTowardsMine,
//             WalkTowardsBase,
//             Mine,
//             Unload
//         }
//
//         public enum Flags
//         {
//             OnTargetReach,
//             OnBagFull,
//             OnBagEmpty
//         }
//
//         public FSM<States, Flags> fsm;
//
//         public Transform baseTransform;
//         public HeadQuartersView hq;
//         public Transform mineTransform;
//
//         public float speed;
//         public float miningSpeed;
//         public float unloadingSpeed;
//         public Inventory inventory;
//         public float proximityThreshold;
//
//         void Start()
//         {
//             fsm = new FSM<States, Flags>(States.WalkTowardsMine);
//
//             fsm.AddState<WalkState>(States.WalkTowardsBase, onTickParams: () => new Func<object>[] { transform, baseTransform, speed, proximityThreshold, Time.deltaTime });
//             fsm.AddState<WalkState>(States.WalkTowardsMine, onTickParams: () => new Func<object>[] { transform, mineTransform, speed, proximityThreshold, Time.deltaTime });
//             fsm.AddState<MineState>(States.Mine, onTickParams: () => new Func<object>[] { inventory, miningSpeed });
//             fsm.AddState<UnloadState>(States.Unload, onTickParams: () => new Func<object>[] { inventory, unloadingSpeed, hq.model });
//
//             fsm.SetTransition(States.WalkTowardsMine, Flags.OnTargetReach, States.Mine, () => { Debug.Log("MineReached"); });
//             fsm.SetTransition(States.Mine, Flags.OnBagFull, States.WalkTowardsBase, () => { Debug.Log("BagFull Returning To Base"); });
//             fsm.SetTransition(States.WalkTowardsBase, Flags.OnTargetReach, States.Unload, () => { Debug.Log("BaseReached"); });
//             fsm.SetTransition(States.Unload, Flags.OnBagEmpty, States.WalkTowardsMine, () => { Debug.Log("Bag Empty Returning to mine"); });
//         }
//
//         // Update is called once per frame
//         void Update()
//         {
//             fsm.Tick();
//         }
//     }
// }