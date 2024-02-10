using System.Collections;
using UnityEngine;

namespace BasicEnemyStateMachine
{
    public class BaseEnemy_AttackState : BaseEnemy_BaseState
    {
        public override void EnterState(BaseEnemy_StateManager state)
        {
            state.animator.Play("Attack", 0, 0);
        }

        public override void UpdateState(BaseEnemy_StateManager state)
        {
            if (state.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f)
                state.SwitchState(state.IdleState);
        }

        public override void ExitState(BaseEnemy_StateManager state)
        {

        }

        public override void OnTriggerEnter(BaseEnemy_StateManager state, Collider other)
        {

        }

        public override void OnTriggerExit(BaseEnemy_StateManager state, Collider other)
        {

        }

        public override void OnTriggerStay(BaseEnemy_StateManager state, Collider other)
        {

        }

    }
}