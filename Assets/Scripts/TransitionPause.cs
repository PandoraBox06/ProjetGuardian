using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionPause : StateMachineBehaviour
{
    public string comboName = "Atk";
    public bool combo1;
    public bool combo2;
    public bool combo3;
    public bool combo4;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        CombatManager.instance.PauseAction();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (CombatManager.instance.isAttacking)
        {
            if (combo1)
                animator.Play(comboName + CombatManager.instance.comboCount1);
            if (combo2)
                animator.Play(comboName + CombatManager.instance.comboCount2);
            if (combo3)
                animator.Play(comboName + CombatManager.instance.comboCount3);
            if (combo4)
                animator.Play(comboName + CombatManager.instance.comboCount4);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        CombatManager.instance.isAttacking = false;
        CombatManager.instance.canParse = false;
        CombatManager.instance.CheckParse();
    }

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
