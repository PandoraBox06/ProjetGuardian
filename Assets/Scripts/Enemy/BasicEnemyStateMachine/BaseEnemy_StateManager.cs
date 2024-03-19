using System;
using UnityEngine;
using UnityEngine.AI;

namespace BasicEnemyStateMachine
{
    public class BaseEnemy_StateManager : MonoBehaviour
    {
        BaseEnemy_BaseState currentState;

        public readonly BaseEnemy_IdleState IdleState = new();
        public readonly BaseEnemy_AttackState AttackState = new();
        public readonly BaseEnemy_TrackPlayer TrackPlayerState = new();
        // public BaseEnemy_Roaming RoamingState = new();
        public readonly BaseEnemy_Stun StunState = new();
        public readonly BaseEnemy_GetInRange GetInRangeState = new();

        [Header("References")]
        [SerializeField] private Enemy_Data enemyData;
        public Animator animator;
        public NavMeshAgent agent;
        [HideInInspector] public GameObject projectiles;
        public Transform fireOutput;
        public Transform projectileDump;
        public Enemy stats;
    
        [Header("Settings")]
        [HideInInspector] public bool trainingDummyMode;
        [HideInInspector] public float idleTime = 1;
        [HideInInspector] public CombatMode combatType;
        [HideInInspector] public float meleeAttackRange;
        [HideInInspector] public float rangeAttackRange = 15;
        [HideInInspector] public float minRangeAttackRange = 7;
        [HideInInspector] public float projectilesSpeed = 10;
        [HideInInspector] public bool isStunned;
        [HideInInspector] public float stunTimer = 1;

        [Header("Hp Floor Percentage (X/100%)")]
        [HideInInspector] [Range(0f, 1f)] public float highHpPercentLower;
        [HideInInspector] [Range(0f, 1f)] public float midHpPercentUpper, midHpPercentLower;
        [HideInInspector] [Range(0f, 1f)] public float lowHpPercentUpper, lowHpPercentLower;

        [Header("Percentage For States High HP")] 
        [HideInInspector] [Range(0f, 1f)] public float highHpPercentMelee;
        [HideInInspector] [Range(0f, 1f)] public float highHpPercentRange;
        [HideInInspector] [Range(0f, 1f)] public float highHpPercentDodge;

        [Header("Percentage For States Mid HP")] 
        [HideInInspector] [Range(0f, 1f)] public float midHpPercentMelee;
        [HideInInspector] [Range(0f, 1f)] public float midHpPercentRange;
        [HideInInspector] [Range(0f, 1f)] public float midHpPercentDodge;

        [Header("Percentage For States Low HP")] 
        [HideInInspector] [Range(0f, 1f)] public float lowHpPercentMelee;
        [HideInInspector] [Range(0f, 1f)] public float lowHpPercentRange;
        [HideInInspector] [Range(0f, 1f)] public float lowHpPercentDodge;

        [Header("Time for the current State")] 
        [HideInInspector] public float randomTimeForMeleeUpper;
        [HideInInspector] public float randomTimeForMeleeLower;
        [HideInInspector] public float randomTimeForRangeUpper;
        [HideInInspector] public float randomTimeForRangeLower;
        [HideInInspector] public float randomTimeForDodgeUpper;
        [HideInInspector] public float randomTimeForDodgeLower;
      
        //Hidden
        [HideInInspector] public Transform player;
        [HideInInspector] public float timer;
        // Discarded but still here if needed
        [HideInInspector] public Transform[] roamingPoints;
        [HideInInspector] public PatrolRoute route;
        [HideInInspector] public bool randomRoaming;
        [HideInInspector] public float roamingTimer = 1;
        [HideInInspector] public bool inverseDirection;
        
        public enum CombatMode
        {
            melee,
            range,
            dodge
        }

        private void Awake()
        {
            enemyData.SetUpStateManager(out projectiles, out trainingDummyMode, out idleTime, out meleeAttackRange,
                out rangeAttackRange, out minRangeAttackRange, out projectilesSpeed, out stunTimer,
                out highHpPercentLower, out midHpPercentUpper, out midHpPercentLower, out lowHpPercentUpper,
                out lowHpPercentLower, out highHpPercentMelee, out highHpPercentRange, out highHpPercentDodge,
                out midHpPercentMelee, out midHpPercentRange, out midHpPercentDodge, out lowHpPercentMelee,
                out lowHpPercentRange, out lowHpPercentDodge, out randomTimeForMeleeUpper, out randomTimeForMeleeLower,
                out randomTimeForRangeUpper, out randomTimeForRangeLower, out randomTimeForDodgeUpper, out randomTimeForDodgeLower);
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

            if(timer > 0)
                timer -= Time.deltaTime;
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
            if (go != this.transform.root.gameObject) return;
            var thisProjectile = Instantiate(projectiles, fireOutput.position, Quaternion.identity, projectileDump);
            var projectileDir = player.position - transform.position;
            thisProjectile.GetComponent<Rigidbody>().AddForce(projectileDir * projectilesSpeed, ForceMode.Impulse);
        }
        
        public void CheckTimer()
        {
            if (timer <= 0)
            {
                SwitchState(IdleState);
            }
        }
    }
}
