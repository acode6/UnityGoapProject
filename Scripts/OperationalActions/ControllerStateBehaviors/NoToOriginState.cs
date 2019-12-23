using System;
using UnityEngine;
using DefenderNameSpace;

public class NoToOriginState : StateMachineBehaviour
{
    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
#if PLANNER_DOMAIN_GENERATED
        var defender = animator.gameObject.GetComponent<Defender>();
        var motionController = animator.gameObject.GetComponent<MotionController>();
        if(motionController.TargetPosition == Vector3.zero)
        {
            Debug.Log("Thought it was");
          defender.CompleteAction();
        }
        
       
#endif
    }
}
