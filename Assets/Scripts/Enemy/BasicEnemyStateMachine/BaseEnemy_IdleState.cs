using UnityEngine;

namespace BasicEnemyStateMachine
{
    public class BaseEnemy_IdleState : BaseEnemy_BaseState
    {
        float timer;

        public override void EnterState(BaseEnemy_StateManager state)
        {
           timer = Time.time + state.idleTime;
        }

        public override void UpdateState(BaseEnemy_StateManager state)
        {
            if(state.trainingDummyMode) { return; }

            if (state.isStunned) { state.SwitchState(state.StunState); }

            if (!(Time.time > timer)) return;
            
            switch (state.timer)
            {
                case <= 0 when state.combatType == BaseEnemy_StateManager.CombatMode.melee:
                    state.timer = Random.Range(state.randomTimeForMeleeLower, state.randomTimeForMeleeUpper);
                    break;
                case <= 0 when state.combatType == BaseEnemy_StateManager.CombatMode.range:
                    state.timer = Random.Range(state.randomTimeForRangeLower, state.randomTimeForRangeUpper);
                    break;
                case <= 0 when state.combatType == BaseEnemy_StateManager.CombatMode.dodge:
                    state.timer = Random.Range(state.randomTimeForDodgeLower, state.randomTimeForDodgeUpper);
                    break;
            }
            GetNextState(state);
            state.SwitchState(state.TrackPlayerState);
        }

        public override void ExitState(BaseEnemy_StateManager state)
        {

        }

        private void GetNextState(BaseEnemy_StateManager state)
        {
            if (state.stats.currentHealth / state.stats.maxHealth >= state.highHpPercentLower)
            {
                float percentage = Random.Range(0, 1f);
                if (percentage >= state.highHpPercentMelee)
                {
                    state.combatType = BaseEnemy_StateManager.CombatMode.melee;
                }
                else if(percentage >= state.highHpPercentRange && percentage < state.highHpPercentMelee)
                {
                    state.combatType = BaseEnemy_StateManager.CombatMode.range;
                }
                else if(percentage >= state.highHpPercentDodge && percentage < state.highHpPercentRange)
                {
                    state.combatType = BaseEnemy_StateManager.CombatMode.dodge;
                }
            }
            else if (state.stats.currentHealth / state.stats.maxHealth >= state.midHpPercentLower && state.stats.currentHealth / state.stats.maxHealth < state.midHpPercentUpper)
            {
                float percentage = Random.Range(0, 1f);
                if (percentage >= state.midHpPercentMelee)
                {
                    state.combatType = BaseEnemy_StateManager.CombatMode.melee;
                }
                else if(percentage >= state.midHpPercentRange && percentage < state.midHpPercentMelee)
                {
                    state.combatType = BaseEnemy_StateManager.CombatMode.range;
                }
                else if(percentage >= state.midHpPercentDodge && percentage < state.midHpPercentRange)
                {
                    state.combatType = BaseEnemy_StateManager.CombatMode.dodge;
                }
            }
            else if (state.stats.currentHealth / state.stats.maxHealth >= state.lowHpPercentLower && state.stats.currentHealth / state.stats.maxHealth < state.lowHpPercentUpper)
            {
                float percentage = Random.Range(0, 1f);
                if (percentage >= state.lowHpPercentMelee)
                {
                    state.combatType = BaseEnemy_StateManager.CombatMode.melee;
                }
                else if(percentage >= state.lowHpPercentRange && percentage < state.lowHpPercentMelee)
                {
                    state.combatType = BaseEnemy_StateManager.CombatMode.range;
                }
                else if(percentage >= state.lowHpPercentDodge && percentage < state.lowHpPercentRange)
                {
                    state.combatType = BaseEnemy_StateManager.CombatMode.dodge;
                }
            }
        }
    }
}