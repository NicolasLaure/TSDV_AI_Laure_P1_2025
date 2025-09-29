using System;
using FSM;
using RTS.Model;

public class HideState : State
{
    public override Type[] OnEnterParametersTypes => new Type[]
    {
        typeof(HeadQuarters),
        typeof(Inventory),
        typeof(bool)
    };

    private HeadQuarters hq;

    public override BehaviourActions GetOnEnterBehaviours(params object[] parameters)
    {
        Inventory inventory = (Inventory)parameters[1];
        bool shouldUnload = (bool)parameters[2];

        BehaviourActions behaviourActions = new BehaviourActions();
        behaviourActions.AddMainThreadBehaviour(0, () =>
        {
            hq = (HeadQuarters)parameters[0];

            inventory.food = inventory.maxFood;
            if (shouldUnload)
            {
                hq.AddResources(inventory.heldResources);
                inventory.heldResources = 0;
            }
        });

        return behaviourActions;
    }
}