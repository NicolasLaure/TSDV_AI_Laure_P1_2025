// using Creeper.States;
// using UnityEngine;
//
// class Agent : MonoBehaviour
// {
//     public enum States
//     {
//         Patrol,
//         Chase,
//         Explode
//     }
//
//     public enum Flags
//     {
//         OnTargetReach,
//         OnTargetNear,
//         OnTargetLost
//     }
//
//     public FSM<States, Flags> fsm;
//
//     public Transform target;
//     public float speed;
//     public float explodeDistance;
//     public float lostDistance;
//
//     public Transform wayPoint1;
//     public Transform wayPoint2;
//     public float chaseDistance;
//
//     public void Start()
//     {
//         fsm = new FSM<States, Flags>(States.Patrol);
//
//         fsm.AddState<PatrolState>(States.Patrol, onTickParams: () => new object[] { wayPoint1, wayPoint2, transform, target, speed, chaseDistance, Time.deltaTime });
//         fsm.AddState<ChaseState>(States.Chase, onTickParams: () => new object[] { transform, target, speed, explodeDistance, lostDistance, Time.deltaTime });
//         fsm.AddState<ExplodeState>(States.Explode, onEnterParams: () => new object[] { transform });
//
//         fsm.SetTransition(States.Patrol, Flags.OnTargetNear, States.Chase, () => { Debug.Log("Chase"); });
//         fsm.SetTransition(States.Chase, Flags.OnTargetReach, States.Explode, () => { Debug.Log("Kaboom"); });
//         fsm.SetTransition(States.Chase, Flags.OnTargetLost, States.Patrol, () => { Debug.Log("Lost"); });
//     }
//
//     private void Update()
//     {
//         fsm.Tick();
//     }
//
//     [ContextMenu("OnSeeTarget")]
//     public void TriggerOnSeeTarget()
//     {
//         fsm.Transition(Flags.OnTargetNear);
//     }
//
//     [ContextMenu("OnTargetReach")]
//     public void TriggerOnTargetReach()
//     {
//         fsm.Transition(Flags.OnTargetReach);
//     }
//
//     [ContextMenu("OnTargetLost")]
//     public void TriggerOnTargetLost()
//     {
//         fsm.Transition(Flags.OnTargetLost);
//     }
// }