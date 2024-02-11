using System.Collections;
using UnityEngine;

namespace BasicEnemyStateMachine
{
    public abstract class BaseEnemy_BaseState
    {
        public abstract void EnterState(BaseEnemy_StateManager state);
        public abstract void UpdateState(BaseEnemy_StateManager state);
        public abstract void ExitState(BaseEnemy_StateManager state);
    }
}