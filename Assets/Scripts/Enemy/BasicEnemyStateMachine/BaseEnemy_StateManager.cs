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
        
        [Header("Settings")]
        public bool trainingDummyMode;
        public float idleTime = 1;
        public CombatMode combatType;
        public float meleeAttackRange = 1.5f;
        public float rangeAttackRange = 15;
        public float minRangeAttackRange = 7;
        public float projectilesSpeed = 10;
        public bool isStunned;
        public float stunTimer = 1;
        [Header("Hp Floor Percentage (X/100%)")]
        [Range(0f, 1f)] public float highHpPercentLower;
        [Range(0f, 1f)] public float midHpPercentUpper, midHpPercentLower;
        [Range(0f, 1f)] public float lowHpPercentUpper, lowHpPercentLower;

        [Header("Percentage For States High HP")] 
        [Range(0f, 1f)] public float highHpPercentMelee;
        [Range(0f, 1f)] public float highHpPercentRange;
        [Range(0f, 1f)] public float highHpPercentDodge;

        [Header("Percentage For States Mid HP")] 
        [Range(0f, 1f)] public float midHpPercentMelee;
        [Range(0f, 1f)] public float midHpPercentRange;
        [Range(0f, 1f)] public float midHpPercentDodge;

        [Header("Percentage For States Low HP")] 
        [Range(0f, 1f)] public float lowHpPercentMelee;
        [Range(0f, 1f)] public float lowHpPercentRange;
        [Range(0f, 1f)] public float lowHpPercentDodge;

        [Header("Time for the current State")] 
        public float randomTimeForMeleeUpper;
        public float randomTimeForMeleeLower;
        public float randomTimeForRangeUpper;
        public float randomTimeForRangeLower;
        public float randomTimeForDodgeUpper;
        public float randomTimeForDodgeLower;
        [Header("References")]
        public Animator animator;
        public NavMeshAgent agent;
        public GameObject projectiles;
        public Transform fireOutput;
        public Transform projectileDump;
        public Enemy stats;
        
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
    }
}
