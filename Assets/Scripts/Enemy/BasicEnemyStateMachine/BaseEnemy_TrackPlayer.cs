using UnityEngine;

namespace BasicEnemyStateMachine
{
    public class BaseEnemy_TrackPlayer : BaseEnemy_BaseState
    {
        float attackRange;

        public override void EnterState(BaseEnemy_StateManager state)
        {
            attackRange = state.combatType switch
            {
                BaseEnemy_StateManager.CombatMode.melee => state.meleeAttackRange,
                BaseEnemy_StateManager.CombatMode.range => state.rangeAttackRange,
                BaseEnemy_StateManager.CombatMode.dodge => 99,
                _ => attackRange
            };

            state.agent.stoppingDistance = attackRange;

            FindingPlayer(state);
        }
        public override void UpdateState(BaseEnemy_StateManager state)
        {
            if (state.isStunned) { state.SwitchState(state.StunState); }

            state.animator.SetFloat("Speed", state.agent.velocity.magnitude);

            FindingPlayer(state);
        }

        private bool IsPlayerInAttackRange(BaseEnemy_StateManager state)
        {
            return Vector3.Distance(state.player.position, state.transform.position) <= attackRange;
        }

        private float PlayerDistance(BaseEnemy_StateManager state)
        {
            return Vector3.Distance(state.player.position, state.transform.position);
        }

        public override void ExitState(BaseEnemy_StateManager state)
        {

        }

        private void FindingPlayer(BaseEnemy_StateManager state)
        {
            if (state.player != null)
            {
                if (state.combatType == BaseEnemy_StateManager.CombatMode.range)
                {
                    if (PlayerDistance(state) < state.minRangeAttackRange)
                    {
                        state.SwitchState(state.GetInRangeState);
                    }
                    else
                    {
                        if (IsPlayerInAttackRange(state))
                            state.SwitchState(state.AttackState);
                        else
                        {
                            state.agent.SetDestination(state.player.position);
                        }
                    }
                }
                else
                {
                    if (IsPlayerInAttackRange(state))
                        state.SwitchState(state.AttackState);
                    else
                    {
                        state.agent.SetDestination(state.player.position);
                    }
                }
            }
            else
                state.SwitchState(state.IdleState);
        }

    }
}