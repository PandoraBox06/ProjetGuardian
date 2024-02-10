using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BasicEnemyStateMachine
{
    public class BaseEnemy_StateManager : MonoBehaviour
    {
        BaseEnemy_BaseState currentState;

        public BaseEnemy_IdleState IdleState = new();
        public BaseEnemy_AttackState AttackState = new();

        [Header("Settings")]
        public float idleTime = 1;

        [Header("References")]
        public Animator animator;

        void Start()
        {
            //Starting by Idle State
            currentState = IdleState;
            //Current State Start Function
            currentState.EnterState(this);
        }

        void Update()
        {
            //CurrentState Update Function
            currentState.UpdateState(this);
        }

        public void SwitchState(BaseEnemy_BaseState state)
        {
            //Switch State and Start Function
            currentState = state;
            state.EnterState(this);
        }

        public void OnTriggerEnter(Collider other)
        {
            currentState.OnTriggerEnter(this, other);
        }

        public void OnTriggerStay(Collider other)
        {
            currentState.OnTriggerStay(this, other);
        }

        public void OnTriggerExit(Collider other)
        {
            currentState.OnTriggerExit(this, other);
        }
    }
}
