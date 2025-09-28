using System;
using System.Collections.Generic;
using FSM;
using RTS.Model;

public class IdleState : State
{
    public override Type[] OnTickParametersTypes => new Type[]
    {
        typeof(Map),
        typeof(Func<WorkerAgent>)
    };

    public override BehaviourActions GetOnTickBehaviours(params object[] parameters)
    {
        Map map = (Map)parameters[0];
        WorkerAgent agent = ((Func<WorkerAgent>)parameters[1]).Invoke();

        BehaviourActions behaviourActions = new BehaviourActions();
        behaviourActions.AddMainThreadBehaviour(0, () => { });

        behaviourActions.SetTransitionBehaviour(() =>
        {
            Mine leastSuppliedMine = null;
            List<Mine> workingMines = new List<Mine>();
            foreach (Mine mine in map.GetMines())
            {
                if (mine.workingVillagers != 0)
                {
                    workingMines.Add(mine);
                }
            }

            foreach (Mine mine in workingMines)
            {
                if (leastSuppliedMine == null)
                    leastSuppliedMine = mine;

                if (mine.suppliers < leastSuppliedMine.suppliers)
                    leastSuppliedMine = mine;
            }

            if (leastSuppliedMine != null)
            {
                agent.closestMineNode = map.GetNode(leastSuppliedMine);
                OnFlag?.Invoke(WorkerAgent.Flags.OnMineEmpty);
            }
        });

        return behaviourActions;
    }
}