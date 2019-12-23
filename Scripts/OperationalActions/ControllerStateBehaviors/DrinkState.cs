
using System;
using UnityEngine;
using DefenderNameSpace;

public class DrinkState : StateMachineBehaviour
{
    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
#if PLANNER_DOMAIN_GENERATED
        var defender = animator.gameObject.GetComponent<Defender>();
        defender.CompleteAction();
        Debug.Log("Just Drank");
#endif
    }
}
