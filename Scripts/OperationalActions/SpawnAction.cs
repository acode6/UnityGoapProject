#if PLANNER_ACTIONS_GENERATED
using System;
using Unity.AI.Planner.DomainLanguage.TraitBased;
using Unity.Collections;
using UnityEngine;
using DefenderNameSpace;
using AI.Planner.Domains;
using AI.Planner.Domains.Enums;

public class SpawnAction : DefenderAction
{
    static readonly int k_Spawn = Animator.StringToHash("Spawn");


    public override void BeginExecution(StateData state, ActionKey action, Defender actor)
    {
        Debug.Log("Spawning Baby");
        base.BeginExecution(state, action, actor);
        AnimationComplete = false;

        var nest = state.GetTraitOnObjectAtIndex<Nest>(action[1]); //Geting Nest index
        


        m_Animator.SetTrigger(k_Spawn); //set the state machine to the depositresource state
    }

    public override void EndExecution(StateData state, ActionKey action, Defender actor)
    {
        base.EndExecution(state, action, actor);
        
        var domainObjects = new NativeList<(DomainObject, int)>(4, Allocator.TempJob);
        foreach (var (_, domainObjectIndex) in state.GetDomainObjects(domainObjects, typeof(Nest))) //get nest for storage
        {
            var nestStorage = state.GetTraitOnObjectAtIndex<Nest>(domainObjectIndex);
            nestStorage.NestStorage -= 10 ; //subtract resouces used for spawn
            state.SetTraitOnObjectAtIndex(nestStorage, domainObjectIndex); //set this
        }

        Debug.Log("Spawn Complete");
        domainObjects.Dispose();
    }
}
#endif
