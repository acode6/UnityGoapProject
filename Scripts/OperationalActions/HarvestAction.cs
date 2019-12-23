#if PLANNER_ACTIONS_GENERATED
using System;
using AI.Planner.Domains;
using AI.Planner.Domains.Enums;
using Unity.AI.Planner.DomainLanguage.TraitBased;
using Unity.Collections;
using Unity.Entities;
using DefenderNameSpace;
using UnityEngine;

public class HarvestAction : DefenderAction
{
    static readonly int k_Harvestable = Animator.StringToHash("Harvest");
    static readonly int k_collectWood = Animator.StringToHash("CollectWood");
   
    HarvestableType m_HarvestableType;

    public override void BeginExecution(StateData state, ActionKey action, Defender actor)
    {
       // Debug.Log("Im Harvesting");
        base.BeginExecution(state, action, actor);

        AnimationComplete = false;
        m_Animator.SetTrigger(k_Harvestable);

        var dispenser = state.GetTraitOnObjectAtIndex<HarvestableTrait>(action[1]);
        // var harvestTar = state.GetTraitOnObjectAtIndex<HarvestableTrait>(action[1]);
        m_HarvestableType = dispenser.HarvestResourse;
        m_Animator.SetTrigger(m_HarvestableType == HarvestableType.Wood ? k_collectWood : k_collectWood);
    }

    public override void EndExecution(StateData state, ActionKey action, Defender actor)
    {
       // Debug.Log("Done Harvesting");
        var harvest = state.GetTraitOnObjectAtIndex<HarvestableTrait>(action[1]);
        var domainObjects = new NativeList<(DomainObject, int)>(4, Allocator.TempJob);
        foreach (var (_, domainObjectIndex) in state.GetDomainObjects(domainObjects, new ComponentType[] { typeof(Pouch) }))
        {
            var pouch = state.GetTraitOnObjectAtIndex<Pouch>(domainObjectIndex);
            pouch.Amount += pouch.HarvestType == m_HarvestableType? harvest.HarvestAmount: 0;
            state.SetTraitOnObjectAtIndex(pouch, domainObjectIndex);
        }
        domainObjects.Dispose();

        base.EndExecution(state, action, actor);
    }
}
#endif