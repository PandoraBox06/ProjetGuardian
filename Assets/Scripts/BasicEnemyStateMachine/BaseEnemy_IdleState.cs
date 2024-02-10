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
                state.SwitchState(state.AttackState);
            }
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