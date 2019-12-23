
using System;
using UnityEngine;
using DefenderNameSpace;

public class CheckNeeds : StateMachineBehaviour
{
    float timer = 0;
    float Eat = 10;
    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        
        var defender = animator.gameObject.GetComponent<Defender>();
        timer += Time.deltaTime;
        Debug.Log("Exploring timer" + timer);
        if (timer > Eat)
        {
            Debug.Log("ending exploration");
            animator.SetBool("atDestination", true);
            timer = 0;
        }
        
        

    }
  
}
