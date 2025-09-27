using System;
using FSM;
using UnityEngine;

namespace RTS.Model
{
    public class MineState : State
    {
        public override Type[] OnTickParametersTypes => new Type[]
        {
            typeof(Mine),
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
            Mine mine = parameters[0] as Mine;
            Inventory inventory = parameters[1] as Inventory;
            float miningSpeed = (float)parameters[2];

            BehaviourActions behaviourActions = new BehaviourActions();
            behaviourActions.AddMainThreadBehaviour(0, () =>
            {
                if (Time.time - startTime >= miningSpeed)
                {
                    startTime = Time.time;
                    if (mine.TryExtract())
                    {
                        inventory.heldResources = Mathf.Clamp(inventory.heldResources + 1, 0, inventory.size);
                    }
                }
            });

            behaviourActions.SetTransitionBehaviour(() =>
            {
                if (inventory.heldResources == inventory.size)
                    OnFlag?.Invoke(WorkerAgent.Flags.OnBagFull);

                if (mine.ShouldRemove)
                    OnFlag?.Invoke(WorkerAgent.Flags.OnMineEmpty);
            });

            return behaviourActions;
        }
    }
}