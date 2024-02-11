using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace BasicEnemyStateMachine
{
    public class BaseEnemy_StateManager : MonoBehaviour
    {
        BaseEnemy_BaseState currentState;

        public BaseEnemy_IdleState IdleState = new();
        public BaseEnemy_AttackState AttackState = new();
        public BaseEnemy_TrackPlayer TrackPlayerState = new();
        public BaseEnemy_Roaming RoamingState = new();

        [Header("Settings")]
        public bool canMove;
        public bool canAttack;
        public float idleTime = 1;
        public float attackRange = 1;
        public bool randomRoaming = false;
        public float roamingTimer = 1;
        public bool inverseDirection;

        [Header("References")]
        public Animator animator;
        public NavMeshAgent agent;
        public Transform[] roamingPoints;
        public PatrolRoute route;
        //Hidden
        /*[HideInInspector]*/ public Transform player;

        void Start()
        {
            //Starting by Idle State
            currentState = IdleState;
            //Current State Start Function
            currentState.EnterState(this);

            if(route != null )
                roamingPoints = route.Waypoints;
        }

        void Update()
        {
            //CurrentState Update Function
            currentState.UpdateState(this);
        }

        public void SwitchState(BaseEnemy_BaseState state)
        {
            //Switch State and Start Function
            ExitState(state);
            currentState = state;
            state.EnterState(this);
        }

        public void ExitState(BaseEnemy_BaseState state)
        {
            state.ExitState(this);
        }
    }
}
