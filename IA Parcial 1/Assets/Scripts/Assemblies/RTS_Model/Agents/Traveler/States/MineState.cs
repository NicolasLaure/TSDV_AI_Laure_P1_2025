using System;
using FSM;
using UnityEngine;

namespace RTS.Model
{
    public class MineState : State
    {
        public override Type[] OnEnterParametersTypes => new Type[]
        {
            typeof(Mine)
        };

        public override Type[] OnTickParametersTypes => new Type[]
        {
            typeof(Inventory),
            typeof(float),
            typeof(Func<Action>)
        };

        private float startTime;
        private Mine mine;

        public override BehaviourActions GetOnEnterBehaviours(params object[] parameters)
        {
            Mine receivedMine = (Mine)parameters[0];
            BehaviourActions behaviourActions = new BehaviourActions();
            behaviourActions.AddMainThreadBehaviour(0, () =>
            {
                startTime = Time.time;
                mine = receivedMine;
                mine.workingVillagers++;
            });
            return behaviourActions;
        }

        public override BehaviourActions GetOnExitBehaviours(params object[] parameters)
        {
            BehaviourActions behaviourActions = new BehaviourActions();
            behaviourActions.AddMainThreadBehaviour(0, () =>
            {
                startTime = Time.time;
                mine.workingVillagers--;
            });
            return behaviourActions;
        }

        public override BehaviourActions GetOnTickBehaviours(params object[] parameters)
        {
            Inventory inventory = parameters[0] as Inventory;
            float miningSpeed = (float)parameters[1];
            Action onFoodUpdated = ((Func<Action>)parameters[2]).Invoke();

            BehaviourActions behaviourActions = new BehaviourActions();
            behaviourActions.AddMainThreadBehaviour(0, () =>
            {
                if (Time.time - startTime >= miningSpeed)
                {
                    startTime = Time.time;
                    if (inventory.food <= 0)
                    {
                        inventory.food = mine.TryGetFood(inventory.maxFood);
                        return;
                    }

                    if (mine.CanExtract() && inventory.food > 0)
                    {
                        mine.Extract();
                        inventory.heldResources = Mathf.Clamp(inventory.heldResources + 1, 0, inventory.size);
                        inventory.food--;
                        onFoodUpdated?.Invoke();
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