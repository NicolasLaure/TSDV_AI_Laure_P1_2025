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
            typeof(float),
            typeof(float)
        };

        private float timer;

        public override BehaviourActions GetOnEnterBehaviours(params object[] parameters)
        {
            BehaviourActions behaviourActions = new BehaviourActions();
            behaviourActions.AddMainThreadBehaviour(0, () => { timer = 0; });
            return behaviourActions;
        }

        public override BehaviourActions GetOnTickBehaviours(params object[] parameters)
        {
            MapEntity mapEntity = parameters[0] as MapEntity;
            Inventory inventory = parameters[1] as Inventory;
            float unloadSpeed = (float)parameters[2];
            float deltaTime = (float)parameters[3];
            BehaviourActions behaviourActions = new BehaviourActions();
            behaviourActions.AddMainThreadBehaviour(0, () =>
            {
                if (timer >= unloadSpeed)
                {
                    timer = 0;
                    mapEntity.AddResources(inventory.heldResources);
                    inventory.heldResources = 0;
                }

                timer += deltaTime;
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