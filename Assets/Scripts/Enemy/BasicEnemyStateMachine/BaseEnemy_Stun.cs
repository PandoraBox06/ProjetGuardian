using System.Collections;
using UnityEngine;

namespace BasicEnemyStateMachine
{
    public class BaseEnemy_Stun : BaseEnemy_BaseState
    {
        float timer;

        public override void EnterState(BaseEnemy_StateManager state)
        {
            timer = Time.time + state.stunTimer;
        }

        public override void UpdateState(BaseEnemy_StateManager state)
        {
            if(Time.time > timer)
            {
                state.SwitchState(state.IdleState);
            }
        }

        public override void ExitState(BaseEnemy_StateManager state)
        {
            state.isStunned = false;
        }
    }
}