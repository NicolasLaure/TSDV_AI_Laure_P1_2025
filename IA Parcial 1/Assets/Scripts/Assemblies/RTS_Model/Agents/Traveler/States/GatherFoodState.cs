using System;
using FSM;
using RTS.Model;
using UnityEngine;

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
    };

    private float startTime;
    private HeadQuarters hq;

    public override BehaviourActions GetOnEnterBehaviours(params object[] parameters)
    {
        HeadQuarters receivedHQ = (HeadQuarters)parameters[0];
        BehaviourActions behaviourActions = new BehaviourActions();
        behaviourActions.AddMainThreadBehaviour(0, () =>
        {
            startTime = Time.time;
            hq = receivedHQ;
        });
        return behaviourActions;
    }

    public override BehaviourActions GetOnTickBehaviours(params object[] parameters)
    {
        Inventory inventory = parameters[0] as Inventory;
        float miningSpeed = (float)parameters[1];

        BehaviourActions behaviourActions = new BehaviourActions();
        behaviourActions.AddMainThreadBehaviour(0, () =>
        {
            if (Time.time - startTime >= miningSpeed)
            {
                startTime = Time.time;
                inventory.heldResources = Mathf.Clamp(inventory.heldResources + hq.GetFood(), 0, inventory.size);
            }
        });

        behaviourActions.SetTransitionBehaviour(() =>
        {
            if (inventory.heldResources == inventory.size)
                OnFlag?.Invoke(WorkerAgent.Flags.OnBagFull);
        });

        return behaviourActions;
    }
}