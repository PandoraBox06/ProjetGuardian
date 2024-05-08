using System;
using UnityEngine;
using UnityEngine.AI;
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

    [SerializeField] private Collider attackBox;
    
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
    bool isReplacing;
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
            if (nextState == Enemy_State.Fire)
            {
                float playerDist = Vector3.Distance(player.position, transform.position);
            
                if (playerDist <= minRangeAttackRange && !isReplacing)
                {
                    //else : go to player
                    agent.stoppingDistance = 1f;
                    animator.SetFloat("Speed", agent.velocity.magnitude);
                    agent.SetDestination(-transform.forward * (minRangeAttackRange + 1));
                    isReplacing = true;
                }
                else
                {
                    isReplacing = false;
                    ChangeState(nextState);
                }
            }
            else
            {
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
        if (!(agent.remainingDistance <= .5f)) return;
        
        transform.LookAt(player);
        //Fire (play anim)
        isAttacking = true;
        animator.Play("Fire");
        //Return to walk
        //Turn towarrds player
    }

    private void Guard()
    {
        if (!isAttacking)
        { 
            //Turn towards player
            transform.LookAt(player);
            //Guard Up
            stats.isGuarding = true;
            animator.SetBool("Block",stats.isGuarding);
            timer = Time.time + stateTimer;
            isAttacking = true;
        }
        else if (Time.time >= timer && stats.isGuarding)
        {
            //if Guard broken : stun
            //Return to walk
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
            stats.ResetGuard();
            animator.SetBool("Stun", isStunned);
            ChangeState(Enemy_State.Walk);
            isStunned = false;
        }
        else
        {
            isStunned = true;
            timer = stunTimer;
            stats.isGuarding = false;
            animator.SetBool("Block",stats.isGuarding);
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
                agent.stoppingDistance = meleeAttackRange;
                toReturn = playerDist <= meleeAttackRange;
                break;
            case Enemy_State.Fire:
                agent.stoppingDistance = rangeAttackRange;
                toReturn = playerDist >= minRangeAttackRange;
                break;
            case Enemy_State.Guard:
                agent.stoppingDistance = meleeAttackRange;
                toReturn = playerDist <= meleeAttackRange;
                break;
            default:
                agent.stoppingDistance = meleeAttackRange;
                toReturn = playerDist <= meleeAttackRange;
                break;
        }

        return toReturn;
    }
    
    private Enemy_State GetNextState()
    {
        Enemy_State toReturn = currentState;
        float percentage = Random.Range(0f, 1f);
        float[] cumulativeProbabilities = new float[3]; // Array to store cumulative probabilities

        // Calculate cumulative probabilities for each behavior based on health percentage ranges
        if (stats.currentHealth / stats.maxHealth >= highHpPercentLower)
        {
            cumulativeProbabilities[0] = highHpPercentMelee;
            cumulativeProbabilities[1] = highHpPercentRange;
            cumulativeProbabilities[2] = highHpPercentDodge;
        }
        else if (stats.currentHealth / stats.maxHealth >= midHpPercentLower && stats.currentHealth / stats.maxHealth < midHpPercentUpper)
        {
            cumulativeProbabilities[0] = midHpPercentMelee;
            cumulativeProbabilities[1] = midHpPercentRange;
            cumulativeProbabilities[2] = midHpPercentDodge;
        }
        else if (stats.currentHealth / stats.maxHealth >= lowHpPercentLower && stats.currentHealth / stats.maxHealth < lowHpPercentUpper)
        {
            cumulativeProbabilities[0] = lowHpPercentMelee;
            cumulativeProbabilities[1] = lowHpPercentRange;
            cumulativeProbabilities[2] = lowHpPercentDodge;
        }

        // Calculate cumulative probabilities
        for (int i = 1; i < cumulativeProbabilities.Length; i++)
        {
            cumulativeProbabilities[i] += cumulativeProbabilities[i - 1];
        }

        // Select behavior based on random number and cumulative probabilities
        if (percentage < cumulativeProbabilities[0])
        {
            // Melee
            toReturn = Enemy_State.Attack;
        }
        else if (percentage < cumulativeProbabilities[1])
        {
            // Range
            toReturn = Enemy_State.Fire;
        }
        else
        {
            // Guard
            toReturn = Enemy_State.Guard;
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

    //Animation EVENT
    public void EnemyEndAnimation()
    {
        if (Time.time >= stateTimer)
        {
            canChangeState = true;
        }

        if (isAttacking) isAttacking = false;
        ChangeState(Enemy_State.Walk);
    }

    public void FireProjectiles()
    {
        if (player != null)
        {
            Instantiate(projectiles, player.position, Quaternion.identity);
        }
    }

    public void EnableAttack()
    {
        attackBox.enabled = true;
    }

    public void DisableAttack()
    {
        attackBox.enabled = false;
    }

    public void WalkEnemySound()
    {
        if(!AudioManager.Instance.walkEnemy.IsNull)
            AudioManager.Instance.PlayOneShot(AudioManager.Instance.walkEnemy, transform.position);
    }
    
    public void GetHitEnemySound()
    {
        if(!AudioManager.Instance.getHitEnemy.IsNull)
            AudioManager.Instance.PlayOneShot(AudioManager.Instance.getHitEnemy, transform.position);
    }
    private void AttackEnemySound()
    {
        if(!AudioManager.Instance.attackEnemy.IsNull)
            AudioManager.Instance.PlayOneShot(AudioManager.Instance.attackEnemy, transform.position);
    }
    public void AttackRangeEnemySound()
    {
        if(!AudioManager.Instance.attackRangeEnemy.IsNull)
           AudioManager.Instance.PlayOneShot(AudioManager.Instance.attackRangeEnemy, transform.position);
    }
    public void GuardEnemySound()
    {
        if(!AudioManager.Instance.guardEnemy.IsNull)
            AudioManager.Instance.PlayOneShot(AudioManager.Instance.guardEnemy, transform.position);
    }
    public void DeathEnemySound()
    {
        if(!AudioManager.Instance.deathEnemy.IsNull)
            AudioManager.Instance.PlayOneShot(AudioManager.Instance.deathEnemy, transform.position);
    }
}
