using System;
using FSM;
using RTS.Model;

public class GatherFoodState : State
{
    public override Type[] OnEnterParametersTypes => new Type[]
    {
        typeof(HeadQuarters)
    };

    public override Type[] OnTickParametersTypes => new Type[]
    {
        typeof(Inventory),
        typeof(float),
        typeof(float)
    };

    private float timer;
    private HeadQuarters hq;

    public override BehaviourActions GetOnEnterBehaviours(params object[] parameters)
    {
        HeadQuarters receivedHQ = (HeadQuarters)parameters[0];
        BehaviourActions behaviourActions = new BehaviourActions();
        behaviourActions.AddMainThreadBehaviour(0, () =>
        {
            timer = 0;
            hq = receivedHQ;
        });
        return behaviourActions;
    }

    public override BehaviourActions GetOnTickBehaviours(params object[] parameters)
    {
        Inventory inventory = parameters[0] as Inventory;
        float miningSpeed = (float)parameters[1];
        float deltaTime = (float)parameters[2];
        BehaviourActions behaviourActions = new BehaviourActions();
        behaviourActions.AddMainThreadBehaviour(0, () =>
        {
            if (timer >= miningSpeed)
            {
                timer = 0;
                inventory.heldResources = Math.Clamp(inventory.heldResources + hq.GetFood(), 0, inventory.size);
            }

            timer += deltaTime;
        });

        behaviourActions.SetTransitionBehaviour(() =>
        {
            if (inventory.heldResources == inventory.size)
                OnFlag?.Invoke(WorkerAgent.Flags.OnBagFull);
        });

        return behaviourActions;
    }
}