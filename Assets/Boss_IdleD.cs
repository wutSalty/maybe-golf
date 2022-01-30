using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_IdleD : StateMachineBehaviour
{
    PlayerHealth healthScript;
    BossPatternsManager patternMan;

    bool PhaseE;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        healthScript = animator.GetComponentInChildren<PlayerHealth>();
        patternMan = animator.GetComponent<BossPatternsManager>();
        patternMan.BeginAttacks();
        healthScript.IFrames = false;
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (healthScript.CurrentHealth <= (healthScript.MaxHealth * 0.05) && !PhaseE)
        {
            PhaseE = true;
            patternMan.PauseAttacks();
            patternMan.bossShoot.PhaseD = false;
            patternMan.bossShoot.PhaseE = true;
            patternMan.BeginAttacks();
        }

        if (healthScript.CurrentHealth <= 0)
        {
            animator.SetBool("Death", true);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        healthScript.IFrames = true;
        patternMan.PauseAttacks();
    }
}
