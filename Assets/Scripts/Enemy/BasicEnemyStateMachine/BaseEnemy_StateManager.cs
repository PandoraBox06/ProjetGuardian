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
        public BaseEnemy_Stun StunState = new();

        [Header("Settings")]
        public bool trainingDummyMode;
        public bool staticAttacking;
        public float idleTime = 1;
        public CombatMode combatType;
        public float meleeAttackRange = 1;
        public float rangeAttackRange = 1;
        public float projectilesSpeed = 10;
        [HideInInspector] public bool randomRoaming = false;
        [HideInInspector] public float roamingTimer = 1;
        [HideInInspector] public bool inverseDirection;
        public bool isStunned = false;
        public float stunTimer = 1;

        [Header("References")]
        public Animator animator;
        public NavMeshAgent agent;
        public Transform player;
        public GameObject projectiles;
        public Transform fireOutput;
        public Transform projectileDump;
        [HideInInspector] public Transform[] roamingPoints;
        [HideInInspector] public PatrolRoute route;
        //Hidden

        public enum CombatMode
        {
            melee,
            range,
            Boss
        }

        private void OnEnable()
        {
            Enemy_AnimatorEvents.OnFireProjectileEnemy += FireProjectile;
        }
        private void OnDisable()
        {
            Enemy_AnimatorEvents.OnFireProjectileEnemy -= FireProjectile;
        }

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


        public void FireProjectile(GameObject go)
        {
            if (go == this.transform.root.gameObject)
            {
                var thisProjectile = Instantiate(projectiles, fireOutput.position, Quaternion.identity, projectileDump);
                Vector3 projectileDir = new();
                projectileDir = player.position - transform.position;
                thisProjectile.GetComponent<Rigidbody>().AddForce(projectileDir * projectilesSpeed, ForceMode.Impulse);
            }
        }
    }
}
