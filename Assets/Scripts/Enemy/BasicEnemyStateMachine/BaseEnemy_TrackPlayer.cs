using System.Collections;
using UnityEngine;

namespace BasicEnemyStateMachine
{
    public class BaseEnemy_TrackPlayer : BaseEnemy_BaseState
    {
        float attackRange;

        public override void EnterState(BaseEnemy_StateManager state)
        {

            switch (state.combatType)
            {
                case BaseEnemy_StateManager.CombatMode.melee:
                    attackRange = state.meleeAttackRange;
                    break;
                case BaseEnemy_StateManager.CombatMode.range:
                    attackRange = state.rangeAttackRange;
                    break;
            }

            state.agent.stoppingDistance = attackRange;

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
            if (Vector3.Distance(state.player.position, state.transform.position) <= attackRange) 
            {
                Debug.Log("True");
                return true;
            }
            else
            {
                Debug.Log("True");
                return false;
            }

        }

        public override void ExitState(BaseEnemy_StateManager state)
        {

        }

    }
}