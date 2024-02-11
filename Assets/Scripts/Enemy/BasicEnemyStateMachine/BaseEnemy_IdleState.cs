using System.Collections;
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
            if(Time.time > timer)
            {
                if (state.player == null) { state.SwitchState(state.RoamingState); }
                else state.SwitchState(state.TrackPlayerState);
            }
        }

        public override void ExitState(BaseEnemy_StateManager state)
        {

        }
    }
}