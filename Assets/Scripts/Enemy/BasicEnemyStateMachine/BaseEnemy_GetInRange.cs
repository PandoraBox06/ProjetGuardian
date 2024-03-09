namespace BasicEnemyStateMachine
{
    public class BaseEnemy_GetInRange : BaseEnemy_BaseState
    {
        float attackRange;
        private bool gotaPath;
        public override void EnterState(BaseEnemy_StateManager state)
        {
            state.agent.stoppingDistance = 0f;
        }

        public override void UpdateState(BaseEnemy_StateManager state)
        {
            if (!state.agent.hasPath && !gotaPath)
            {
                gotaPath = true;
                state.agent.SetDestination(-state.transform.forward * state.minRangeAttackRange);
            }

            if (state.agent.remainingDistance <= 0.1f)
            {
                state.SwitchState(state.TrackPlayerState);
                gotaPath = false;
            }
            
            state.animator.SetFloat("Speed", state.agent.velocity.magnitude);
        }

        public override void ExitState(BaseEnemy_StateManager state)
        {
            attackRange = state.combatType switch
            {
                BaseEnemy_StateManager.CombatMode.melee => state.meleeAttackRange,
                BaseEnemy_StateManager.CombatMode.range => state.rangeAttackRange,
                BaseEnemy_StateManager.CombatMode.dodge => 99,
                _ => attackRange
            };

            state.agent.stoppingDistance = attackRange;
        }
    }
}