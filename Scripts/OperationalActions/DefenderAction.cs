#if PLANNER_ACTIONS_GENERATED
using System;
using System.Collections.Generic;
using AI.Planner.Domains;
using AI.Planner.Domains.Enums;
using Unity.AI.Planner.Agent;
using Unity.AI.Planner.DomainLanguage.TraitBased;
using Unity.Collections;
using DefenderNameSpace;
using UnityEngine;
using Time = UnityEngine.Time;
public abstract class DefenderAction : IOperationalAction<Defender, StateData, ActionKey>
{
    public bool AnimationComplete { get; set; } //for animations

    protected int m_AccumulatedTime;
    protected Animator m_Animator;
    protected Rigidbody m_RigidBody;
    protected float m_MinUnitTime = 1f;
    protected RandomPoint randTarget;
    //dictionary flags to play the death animations
    static readonly Dictionary<NeedType, int> m_DeathAnimationFlags = new Dictionary<NeedType, int>
    {
        { NeedType.Hunger, Animator.StringToHash("Death_NoFood") },
        { NeedType.Thirst, Animator.StringToHash("Death_NoWater") },

    };

    protected float m_StartTime;

    public virtual void BeginExecution(StateData state, ActionKey action, Defender actor)
    {
        //beginning to play animation
        m_StartTime = Time.time;
        m_AccumulatedTime = 0; //beginning of the execution time
        m_Animator = actor.GetComponentInParent<Animator>(); //Get the animator component
        m_RigidBody = actor.GetComponentInParent<Rigidbody>(); //get rigidbody component
        randTarget = actor.GetComponentInParent<RandomPoint>(); // random point geration
        AnimationComplete = false;
    }

    public virtual void ContinueExecution(StateData state, ActionKey action, Defender actor)
    {
        UpdateNeeds(state, actor); //update the need heuristics 
    }

    public virtual void EndExecution(StateData state, ActionKey action, Defender actor)
    {
        UpdateNeeds(state, actor);
    }
    //if operational status is complete or still in progress
    public virtual OperationalActionStatus Status(StateData state, ActionKey action, Defender actor)
    {
        return AnimationComplete && Time.time - m_StartTime > m_MinUnitTime ?
            OperationalActionStatus.Completed : OperationalActionStatus.InProgress;
    }

    void UpdateNeeds(StateData state, Defender actor)
    {
        if (Mathf.Floor(Time.time - m_StartTime) > m_AccumulatedTime)
        {
            m_AccumulatedTime++;

            var domainObjects = new NativeList<(DomainObject, int)>(4, Allocator.TempJob);
            foreach (var (_, domainObjectIndex) in state.GetDomainObjects(domainObjects, typeof(AI.Planner.Domains.Time)))
            {
                var time = state.GetTraitOnObjectAtIndex<AI.Planner.Domains.Time>(domainObjectIndex);
                time.Value += 1; //updating the time value on the action
                state.SetTraitOnObjectAtIndex(time, domainObjectIndex);
            }

            // Resources
            domainObjects.Clear();
            //updating need base on need urgency set in scene view 
            foreach (var (_, domainObjectIndex) in state.GetDomainObjects(domainObjects, typeof(Need)))
            {
                var need = state.GetTraitOnObjectAtIndex<Need>(domainObjectIndex);
                need.Urgency += need.ChangePerSecond;
                state.SetTraitOnObjectAtIndex(need, domainObjectIndex);
                if(need.NeedType == NeedType.Hunger)
                {
                 // Debug.Log("Hunger Urgency" + need.Urgency);
                }
                else if (need.NeedType == NeedType.Thirst)
                {
                 //   Debug.Log("Water Urgency" + need.Urgency);
                }
                
                // Check for death.
                if (need.Urgency > 200)
                {

                    actor.Dead = true;
                    m_Animator.SetBool(m_DeathAnimationFlags[need.NeedType], true);
                }
            }
            domainObjects.Clear();
            

            domainObjects.Dispose();
        }
    }
}
#endif