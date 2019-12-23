#if PLANNER_ACTIONS_GENERATED
using System;
using AI.Planner.Domains;
using AI.Planner.Domains.Enums;
using Unity.AI.Planner.DomainLanguage.TraitBased;
using Unity.Collections;
using Unity.Entities;
using DefenderNameSpace;
using UnityEngine;

public class PickupAction : DefenderAction
{
    static readonly int k_Consumables = Animator.StringToHash("Consumables");
    static readonly int k_PocketFood = Animator.StringToHash("PocketFood");
    static readonly int k_PocketWater = Animator.StringToHash("PocketWater");
    ConsumableType m_ConsumableType;

    public override void BeginExecution(StateData state, ActionKey action, Defender actor)
    {
       
        base.BeginExecution(state, action, actor);

        AnimationComplete = false;
        m_Animator.SetTrigger(k_Consumables);

        var dispenser = state.GetTraitOnObjectAtIndex<HarvestableTrait>(action[1]);
       // var harvestTar = state.GetTraitOnObjectAtIndex<HarvestableTrait>(action[1]);
        m_ConsumableType = dispenser.HarvestableType;
        m_Animator.SetTrigger(m_ConsumableType == ConsumableType.Food ? k_PocketFood : k_PocketWater);
    }

    public override void EndExecution(StateData state, ActionKey action, Defender actor)
    {
       
        var domainObjects = new NativeList<(DomainObject, int)>(4, Allocator.TempJob);
        foreach (var (_, domainObjectIndex) in state.GetDomainObjects(domainObjects, new ComponentType[] { typeof(Inventory) }))
        {
            var inventory = state.GetTraitOnObjectAtIndex<Inventory>(domainObjectIndex);
            inventory.Amount += inventory.ConsumableType == m_ConsumableType ? 2 : 0;
            state.SetTraitOnObjectAtIndex(inventory, domainObjectIndex);
        }
        domainObjects.Dispose();

        base.EndExecution(state, action, actor);
    }
}
#endif
