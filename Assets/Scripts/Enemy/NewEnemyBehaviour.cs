using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;


public class NewEnemyBehaviour : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Enemy_Data _enemyData;
    public Transform Player;
    [SerializeField] private Animator _animator;
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private Enemy _stats;
    
    [SerializeField] private Collider _attackBox;
    [SerializeField] private ParticleSystem _stunEffect;
    [SerializeField] private ParticleSystem _spawnEffect;
    [SerializeField] private float _timer;
    private bool _setTime;
    private bool _isStunned;
    private EnemyState State { get; set; }
    private bool _canMove;

    private void Start()
    {
        State = EnemyState.Idle;
        _animator.Play("spawning");
        _spawnEffect.Play();
        _agent.speed = _enemyData.WalkingSpeed;
        ChangeState(State);
    }

    private bool replacing;
    private void Update()
    {
        if (!_canMove) return;
        if(Player == null)return;
        
        _animator.SetFloat("Speed", _agent.velocity.magnitude);
        switch (State)
        {
            case EnemyState.Idle:
                _setTime = false;
                ChangeState(EnemyState.GoToPlayer);
                break;
            case EnemyState.GoToPlayer:
                GoToPlayer();
                break;
            case EnemyState.Attack:
                if (Vector3.Distance(transform.position, Player.position) <= _enemyData.MeleeAttackRange)
                {
                    TimerHandler();
                    Attack();
                }
                else
                {
                    _agent.SetDestination(Player.position);
                }
                break;
            case EnemyState.Range:
                if (Vector3.Distance(transform.position, Player.position) <= _enemyData.RangeAttackRange && Vector3.Distance(transform.position, Player.position) >= _enemyData.MinRangeAttackRange)
                {
                    TimerHandler();
                    Fire();
                }
                else if (replacing)
                {
                    TimerHandler();
                    Fire();
                }
                else
                {
                    _agent.SetDestination(-transform.forward * (_enemyData.MinRangeAttackRange + 0.5f));
                    replacing = true;
                }
                break;
            case EnemyState.Guard:
                Guard();
                break;
            case EnemyState.Stun:
                Stun();
                break;
        }
    }

    public void ChangeState(EnemyState newState)
    {
        if(Player == null)return;
        if (_enemyData.TrainingDummyMode) return;
        
        State = newState;
    }

    private void GoToPlayer()
    {   
        if(Player == null)return;
        
        if (Vector3.Distance(transform.position, Player.position) > _enemyData.MeleeAttackRange)
        {
            _agent.SetDestination(Player.position);
        }
        else
        {
            ChangeState(EnemyState.Attack);
        }
    }

    private void Attack()
    {
        if (!(Time.time > _timer)) return;
        if(_animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.9f) return;
        int randomAttack = Random.Range(0, 2);
        //Play attack 1 ou 2
        transform.LookAt(Player);
        switch (randomAttack)
        {
            case 0:
                _animator.Play("Attack1");
                break;
            case 1:
                _animator.Play("Attack2");
                break;
        }
        _timer = _enemyData.AttackCooldown;
    }

    private void Fire()
    {
        replacing = false;
        if(_animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.9f) return;
        transform.LookAt(Player);
        _animator.Play("Fire");
    }
    
    private void Guard()
    {
        if(_stats.isGuarding) return;
        //Turn towards player
        transform.LookAt(Player);
        _agent.SetDestination(Player.position);
        if(_agent.remainingDistance > _enemyData.MeleeAttackRange) return;
        //Guard Up
        _stats.isGuarding = true;
        _animator.SetBool("Block",_stats.isGuarding);
        StopCoroutine(GuardTimer());
        StartCoroutine(GuardTimer());
        //if Guard broken : stun
        //Return to walk

    }

    private void Stun()
    {
        if (_isStunned)
        {
            _timer -= Time.deltaTime;
            if (!(_timer <= 0)) return;
            _stats.ResetGuard();
            _isStunned = false;
            _animator.SetBool("Stun", _isStunned);
            ChangeState(EnemyState.Idle);
        }
        else
        {
            StopCoroutine(StateTimer(0));
            _timer = _enemyData.StunTimer;
            _stunEffect.Play();
            _stats.isGuarding = false;
            _animator.SetBool("Block",_stats.isGuarding);
            _isStunned = true;
            _animator.SetBool("Stun", _isStunned);
        }
    }

    private List<Enemy_Data.RandomBehaviour> GetBehaviours()
    {
        List<Enemy_Data.RandomBehaviour> tmpList = new List<Enemy_Data.RandomBehaviour>();
        
        if (GetActualHealth() >= _enemyData.HighHpPercent)
        {
            tmpList = _enemyData.FullLifeBehaviours;
        }

        if (GetActualHealth() <= _enemyData.MidHpPercent)
        {
            tmpList = _enemyData.MidLifeBehaviours;
        }

        if (GetActualHealth() <= _enemyData.LowHpPercent)
        {
            tmpList = _enemyData.LowLifeBehaviours;
        }

        return tmpList;
    }
    private float GetActualHealth()
    {
        return (_stats.currentHealth / _stats.maxHealth) * 100;
    }

    private void TimerHandler()
    {
        if (_setTime) return;
        switch (State)
        {
            case EnemyState.Attack:
                float a = Random.Range(_enemyData.RandomTimeForMeleeLower, _enemyData.RandomTimeForMeleeUpper);
                StopCoroutine(StateTimer(0));
                StartCoroutine(StateTimer(a));
                _setTime = true;
                break;
            case EnemyState.Range:
                float r = Random.Range(_enemyData.RandomTimeForRangeLower, _enemyData.RandomTimeForRangeUpper);
                StopCoroutine(StateTimer(0));
                StartCoroutine(StateTimer(r));
                _setTime = true;
                break;
            case EnemyState.Guard:
            case EnemyState.Idle:
            case EnemyState.GoToPlayer:
            case EnemyState.Stun:
            default:
                return;
        }
    }
    
    private IEnumerator StateTimer(float Ttime)
    {
        yield return new WaitForSeconds(Ttime);
        _enemyData.RandomBehaviours = GetBehaviours();
        ChangeState(_enemyData.GetBehaviour());
        _setTime = false;
        StopCoroutine(StateTimer(0));
    }

    private IEnumerator GuardTimer()
    {
        float g = Random.Range(_enemyData.RandomTimeForDodgeLower, _enemyData.RandomTimeForDodgeUpper);
        StartCoroutine(StateTimer(g));
        _setTime = true;
        yield return new WaitForSeconds(g);
        _stats.isGuarding = false;
        _animator.SetBool("Block",_stats.isGuarding);
        _enemyData.RandomBehaviours = GetBehaviours();
        ChangeState(_enemyData.GetBehaviour());
        _setTime = false;
        StopCoroutine(GuardTimer());
    }
    //ANIMATION EVENT
    public void CanMove()
    {
        _canMove = true;
        ChangeState(State);
    }
    public void FireProjectiles()
    {
        if (Player != null)
        {
            Instantiate(_enemyData.Projectiles, Player.position, Quaternion.identity);
        }
    }

    public void EnableAttack()
    {
        _attackBox.enabled = true;
    }

    public void DisableAttack()
    {
        _attackBox.enabled = false;
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

public enum EnemyState
{
    Idle,
    GoToPlayer,
    Attack,
    Range,
    Guard,
    Stun,
}