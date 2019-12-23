﻿using System;
using UnityEngine;
using DefenderNameSpace;

//To check if you're at the target
public class CheckPosition : StateMachineBehaviour
{
    MotionController motionController;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        motionController = animator.GetComponent<MotionController>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Debug.Log("In check position");
        // Quick calculation for turns
        var currentDirection = animator.transform.forward;
        var targetDirection = motionController.TargetOrientation;

        float angle = Vector3.SignedAngle(currentDirection, targetDirection, Vector3.up);
        animator.SetFloat(Animator.StringToHash("Turn"), angle / 90.0f);

        animator.transform.Rotate(Vector3.up, angle * Time.deltaTime);

        animator.SetBool("InPosition", Mathf.Abs(angle) < 3);
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);
#if PLANNER_DOMAIN_GENERATED
        animator.gameObject.GetComponentInChildren<Defender>().CompleteAction();
#endif
    }
}
