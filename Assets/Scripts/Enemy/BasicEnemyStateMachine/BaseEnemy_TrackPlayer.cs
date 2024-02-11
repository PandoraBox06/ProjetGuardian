using System.Collections;
using UnityEngine;

namespace BasicEnemyStateMachine
{
    public class BaseEnemy_TrackPlayer : BaseEnemy_BaseState
    {
        public override void EnterState(BaseEnemy_StateManager state)
        {
            if (state.player != null)
            {
                if (IsPlayerInAttackRange(state))
                    state.SwitchState(state.AttackState);
                else
                    state.agent.SetDestination(state.player.position);
            }
            else
                state.SwitchState(state.IdleState);
        }
        public override void UpdateState(BaseEnemy_StateManager state)
        {
            if (state.player != null)
            {
                if (IsPlayerInAttackRange(state))
                    state.SwitchState(state.AttackState);
                else
                    state.agent.SetDestination(state.player.position);
            }
            else
                state.SwitchState(state.IdleState);
        }

        public bool IsPlayerInAttackRange(BaseEnemy_StateManager state)
        {
            if(Vector3.Distance(state.player.position, state.transform.position) <= state.attackRange)
            {
                return true;
            }
            else
                return false;
        }

        public override void ExitState(BaseEnemy_StateManager state)
        {

        }

    }
}