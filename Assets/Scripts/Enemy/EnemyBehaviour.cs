using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public enum Enemy_State
{
    Idle,
    Walk,
    Attack,
    Fire,
    Guard,
    Stun
}

public class EnemyBehaviour : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Enemy_Data enemyData;
    [SerializeField] private Enemy_State currentState;
    [SerializeField] private Enemy_State nextState;
    public Transform player;
    [SerializeField] private Animator animator;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform fireOutput;
    [SerializeField] private Transform projectileDump;
    [SerializeField] private Enemy stats;
    
    [Header("Settings")]
    [HideInInspector] public bool trainingDummyMode;
    [HideInInspector] public float idleTime = 1;
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
    private Action DoAction;
    [SerializeField] private float timer;
    [SerializeField] private float stateTimer;
    [HideInInspector] public GameObject projectiles;
    private bool canChangeState;
    
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


    private void Start()
    {
        animator.Play("spawning");
        ChangeState(Enemy_State.Walk);
    }

    private void Update()
    {
        DoAction();
    }


    
    private void Idle()
    {
        
    }

    private void Walk()
    {
        //if close : random behaviour : switch to attack mode or guard
        isAttacking = false;
        
        if (canChangeState)
        {
            nextState = GetNextState();
            canChangeState = false;
        }


        if (player != null)
        {
            //Search if close to player
            if (!CheckDistanceToPlayer())
            {
                //else : go to player
                animator.SetFloat("Speed", agent.velocity.magnitude);
                agent.SetDestination(player.transform.position);
            }
            else
            {
                ChangeState(nextState);
            }
        }
    }

    private bool isAttacking;
    private void Attack()
    {
        if (isAttacking) return;
        //Turn towards player
        transform.LookAt(player);
        //Check again if range
        if (player != null)
        {
            //Search if close to player
            if (!CheckDistanceToPlayer())
            {
                //else : go to player
                animator.SetFloat("Speed", agent.velocity.magnitude);
                agent.SetDestination(player.transform.position);
            }
            else
            {
                isAttacking = true;
                //Random between 1&2
                int randomAttack = Random.Range(0, 2);
                Debug.Log($"Random Attack is : {randomAttack}");
                //Play attack 1 ou 2
                switch (randomAttack)
                {
                    case 0:
                        animator.Play("Attack1");
                        break;
                    case 1:
                        animator.Play("Attack2");
                        break;
                }
                //Return to Walk
            }
        }
    }

    private void Fire()
    {
        if (isAttacking) return;
        //Turn towarrds player
        transform.LookAt(player);
        //Fire (play anim)
        isAttacking = true;
        animator.Play("Fire");
        //Return to walk
    }

    private void Guard()
    {
        //Turn towards player
        transform.LookAt(player);
        //Guard Up
        stats.isGuarding = true;
        animator.SetBool("Block",stats.isGuarding);
        //if Guard broken : stun
        //Return to walk
        if (Time.time >= stateTimer)
        {
            stats.isGuarding = false;
            animator.SetBool("Block",stats.isGuarding);
            ChangeState(Enemy_State.Walk);
        }
    }

    private void Stun()
    {
        //stun cd then return to walk
        if (isStunned)
        {
            timer -= Time.deltaTime;
            if (!(timer <= 0)) return;
            isStunned = false;
            stats.ResetGuard();
            animator.SetBool("Stun", isStunned);
            ChangeState(Enemy_State.Walk);
        }
        else
        {
            isStunned = true;
            timer = stunTimer;
            animator.SetBool("Stun", isStunned);
        }
        
    }

    public void ChangeState(Enemy_State newState)
    {
        if (newState == currentState) return;

        canChangeState = true;
        
        currentState = newState;
        DoAction = currentState switch
        {
            Enemy_State.Idle => Idle,
            Enemy_State.Walk => Walk,
            Enemy_State.Attack => Attack,
            Enemy_State.Fire => Fire,
            Enemy_State.Guard => Guard,
            Enemy_State.Stun => Stun,
            _ => DoAction
        };
        
        SetTimer();
    }
    
    private bool CheckDistanceToPlayer()
    {
        bool toReturn;
        float playerDist = Vector3.Distance(player.position, transform.position);
        switch (currentState)
        {
            case Enemy_State.Attack:
                toReturn = playerDist <= meleeAttackRange;
                break;
            case Enemy_State.Fire:
                toReturn = playerDist <= rangeAttackRange;
                break;
            case Enemy_State.Guard:
                toReturn = playerDist <= meleeAttackRange;
                break;
            default:
                toReturn = playerDist <= meleeAttackRange;
                break;
        }

        return toReturn;
    }
    
    private Enemy_State GetNextState()
    {
        Enemy_State toReturn = currentState;
        float percentage = Random.Range(0, 1f);
        if (stats.currentHealth / stats.maxHealth >= highHpPercentLower)
        {
            if (percentage >= highHpPercentMelee)
            {
                //Melee
                toReturn = Enemy_State.Attack;
            }
            else if(percentage >= highHpPercentRange && percentage < highHpPercentMelee)
            {
                //Range
                toReturn = Enemy_State.Fire;
            }
            else if(percentage >= highHpPercentDodge && percentage < highHpPercentRange)
            {
                //Guard
                toReturn = Enemy_State.Guard;
            }
        }
        else if (stats.currentHealth / stats.maxHealth >= midHpPercentLower && stats.currentHealth / stats.maxHealth < midHpPercentUpper)
        {
            if (percentage >= midHpPercentMelee)
            {
                //Melee
                toReturn = Enemy_State.Attack;
            }
            else if(percentage >= midHpPercentRange && percentage < midHpPercentMelee)
            {
                //Range
                toReturn = Enemy_State.Fire;
            }
            else if(percentage >= midHpPercentDodge && percentage < midHpPercentRange)
            {
               //Guard
               toReturn = Enemy_State.Guard;
            }
        }
        else if (stats.currentHealth / stats.maxHealth >= lowHpPercentLower && stats.currentHealth / stats.maxHealth < lowHpPercentUpper)
        {
            if (percentage >= lowHpPercentMelee)
            {
                //Melee
                toReturn = Enemy_State.Attack;
            }
            else if(percentage >= lowHpPercentRange && percentage < lowHpPercentMelee)
            {
                //Range
                toReturn = Enemy_State.Fire;
            }
            else if(percentage >= lowHpPercentDodge && percentage < lowHpPercentRange)
            {
               //Guard
               toReturn = Enemy_State.Guard;
            }
        }

        return toReturn;
    }
    
    private void SetTimer()
    {
        stateTimer = currentState switch
        {
            Enemy_State.Idle => idleTime,
            Enemy_State.Attack => Random.Range(randomTimeForMeleeLower,randomTimeForMeleeUpper),
            Enemy_State.Fire => Random.Range(randomTimeForRangeLower,randomTimeForRangeUpper),
            Enemy_State.Guard => Random.Range(randomTimeForDodgeLower,randomTimeForDodgeUpper),
            _ => stateTimer
        };
    }

    public void EnemyEndAnimation()
    {
        if (Time.time >= stateTimer)
        {
            canChangeState = true;
        }

        if (isAttacking) isAttacking = false;
        ChangeState(Enemy_State.Walk);
    }
}
