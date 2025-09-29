using System;
using FSM;

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
            typeof(Func<Action>),
            typeof(float)
        };

        private float timer;
        private Mine mine;

        public override BehaviourActions GetOnEnterBehaviours(params object[] parameters)
        {
            Mine receivedMine = (Mine)parameters[0];

            BehaviourActions behaviourActions = new BehaviourActions();
            behaviourActions.AddMainThreadBehaviour(0, () =>
            {
                timer = 0;
                mine = receivedMine;
                mine.workingVillagers++;
            });
            return behaviourActions;
        }

        public override BehaviourActions GetOnExitBehaviours(params object[] parameters)
        {
            BehaviourActions behaviourActions = new BehaviourActions();
            behaviourActions.AddMainThreadBehaviour(0, () => { mine.workingVillagers--; });
            return behaviourActions;
        }

        public override BehaviourActions GetOnTickBehaviours(params object[] parameters)
        {
            Inventory inventory = (Inventory)parameters[0];
            float miningSpeed = (float)parameters[1];
            Action onFoodUpdated = ((Func<Action>)parameters[2]).Invoke();
            float deltaTime = (float)parameters[3];
            BehaviourActions behaviourActions = new BehaviourActions();
            behaviourActions.AddMainThreadBehaviour(0, () =>
            {
                if (timer >= miningSpeed)
                {
                    timer = 0;
                    if (inventory.food <= 0)
                    {
                        inventory.food = mine.TryGetFood(inventory.maxFood);
                        if (inventory.food != 0)
                            onFoodUpdated?.Invoke();
                    }
                    else
                    {
                        if (mine.CanExtract())
                        {
                            mine.Extract();
                            inventory.heldResources = Math.Clamp(inventory.heldResources + 1, 0, inventory.size);
                            inventory.food = Math.Clamp(inventory.food - 1, 0, inventory.maxFood);
                            onFoodUpdated?.Invoke();
                        }
                    }
                }

                timer += deltaTime;
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