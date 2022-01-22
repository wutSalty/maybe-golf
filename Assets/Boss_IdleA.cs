using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_IdleA : StateMachineBehaviour
{
    PlayerHealth healthScript;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        healthScript = animator.GetComponentInChildren<PlayerHealth>();
        healthScript.IFrames = false;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (healthScript.CurrentHealth <= (healthScript.MaxHealth * 0.75))
        {
            animator.SetBool("PhaseB", true);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        healthScript.IFrames = true;
    }
}
