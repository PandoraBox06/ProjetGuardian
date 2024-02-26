using System.Collections;
using UnityEngine;

namespace BasicEnemyStateMachine
{
    public class BaseEnemy_AttackState : BaseEnemy_BaseState
    {
        public override void EnterState(BaseEnemy_StateManager state)
        {
            switch (state.combatType)
            {
                case BaseEnemy_StateManager.CombatMode.melee:
                    state.transform.LookAt(state.player);
                    state.animator.Play("Attack", 0, 0);
                    break;
                case BaseEnemy_StateManager.CombatMode.range:
                    state.transform.LookAt(state.player);
                    state.animator.Play("Shoot", 0, 0);
                    break;
                case BaseEnemy_StateManager.CombatMode.Boss:
                    break;
            }

    
        }

        public override void UpdateState(BaseEnemy_StateManager state)
        {
            if (state.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f)
                state.SwitchState(state.IdleState);
        }

        public override void ExitState(BaseEnemy_StateManager state)
        {

        }
    }
}