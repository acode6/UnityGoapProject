#if PLANNER_ACTIONS_GENERATED
using System;
using Unity.AI.Planner.DomainLanguage.TraitBased;
using Unity.Collections;
using UnityEngine;
using DefenderNameSpace;
using AI.Planner.Domains;
using AI.Planner.Domains.Enums;

public class DepositResource : DefenderAction
{
    static readonly int k_Deposit = Animator.StringToHash("DepositResource");
   

    HarvestableType m_HarvestType;
    //NeedType m_NeedType;
    //long m_NeedReduction;


    public override void BeginExecution(StateData state, ActionKey action, Defender actor)
    {
        Debug.Log("Depositing Resourse");
        base.BeginExecution(state, action, actor);
        AnimationComplete = false;

        var nest = state.GetTraitOnObjectAtIndex<Nest>(action[1]); //Geting Nest index
        //m_NeedType = nest.SatisfiesNeed; //get the need the nest satisfies in this case the need to harvest
        //m_NeedReduction = nest.NeedReduction; //how much it reduces the need


        m_Animator.SetTrigger(k_Deposit); //set the state machine to the depositresource state
    }

    public override void EndExecution(StateData state, ActionKey action, Defender actor)
    {
        base.EndExecution(state, action, actor);
        var pouch = state.GetTraitOnObjectAtIndex<Pouch>(action[2]);
        var domainObjects = new NativeList<(DomainObject, int)>(4, Allocator.TempJob);
        foreach (var (_, domainObjectIndex) in state.GetDomainObjects(domainObjects, typeof(Nest))) //get nest for storage
        {
            var nestStorage = state.GetTraitOnObjectAtIndex<Nest>(domainObjectIndex);
            nestStorage.NestStorage += pouch.Amount; //add the nest storage amount to the pouch amount
            state.SetTraitOnObjectAtIndex(nestStorage, domainObjectIndex); //set this
        }

        domainObjects.Clear();
        foreach (var (_, domainObjectIndex) in state.GetDomainObjects(domainObjects, typeof(Pouch))) //get pouch to set it to zero
        {
            var pouch_ = state.GetTraitOnObjectAtIndex<Pouch>(domainObjectIndex);
            pouch_.Amount = 0; //set the pouch to zero since you've deposited
            state.SetTraitOnObjectAtIndex(pouch_, domainObjectIndex);
        }

       /* foreach (var (_, domainObjectIndex) in state.GetDomainObjects(domainObjects, typeof(Need)))
        {
            var need = state.GetTraitOnObjectAtIndex<Need>(domainObjectIndex);
            need.Urgency -= need.NeedType == m_NeedType ? m_NeedReduction : 0;
            state.SetTraitOnObjectAtIndex(need, domainObjectIndex);
        }*/
       
        Debug.Log("Resource Deposited");
        domainObjects.Dispose();
    }
}
#endif
