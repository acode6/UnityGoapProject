﻿#if PLANNER_ACTIONS_GENERATED
using System;
using UnityEngine;
using DefenderNameSpace;

public class HarvestState : StateMachineBehaviour
{
    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
     
        var defender = animator.gameObject.GetComponent<Defender>();
        var action = (HarvestAction)defender.CurrentOperationalAction;
        action.AnimationComplete = true;
       
    }
}
#endif
