using System;
using FSM;
using UnityEngine;

namespace RTS.Model
{
    public class UnloadState : State
    {
        public override Type[] OnTickParametersTypes => new Type[]
        {
            typeof(MapEntity),
            typeof(Inventory),
            typeof(float)
        };

        private float startTime;

        public override BehaviourActions GetOnEnterBehaviours(params object[] parameters)
        {
            BehaviourActions behaviourActions = new BehaviourActions();
            behaviourActions.AddMainThreadBehaviour(0, () => { startTime = Time.time; });
            return behaviourActions;
        }

        public override BehaviourActions GetOnTickBehaviours(params object[] parameters)
        {
            MapEntity mapEntity = parameters[0] as MapEntity;
            Inventory inventory = parameters[1] as Inventory;
            float unloadSpeed = (float)parameters[2];

            BehaviourActions behaviourActions = new BehaviourActions();
            behaviourActions.AddMainThreadBehaviour(0, () =>
            {
                if (Time.time - startTime >= unloadSpeed)
                {
                    startTime = Time.time;
                    mapEntity.AddResources(inventory.heldResources);
                    inventory.heldResources = 0;
                }
            });

            behaviourActions.SetTransitionBehaviour(() =>
            {
                if (inventory.heldResources == 0)
                    OnFlag?.Invoke(WorkerAgent.Flags.OnBagEmpty);
            });

            return behaviourActions;
        }
    }
}