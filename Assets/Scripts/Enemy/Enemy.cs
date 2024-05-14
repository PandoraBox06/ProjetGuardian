using System;
using BasicEnemyStateMachine;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using Debug = System.Diagnostics.Debug;

public class Enemy : MonoBehaviour, IDamageable
{
    //public
    [SerializeField] private Enemy_Data enemyData;
    [HideInInspector] public float currentHealth;
    public float guardHealth;
    [SerializeField] NewEnemyBehaviour enemyBehaviour;
    [SerializeField] private Animator animator;
    //private
    [SerializeField] private GameObject hpEnemy;
    [SerializeField] private SpriteRenderer barSpriteRenderer;
    [SerializeField] private Transform barParent;
    [HideInInspector] public float maxHealth;
    [SerializeField] private Gradient hpGradient;
    public bool isGuarding;
    
    [Header("VFX")]
    [HideInInspector] public GameObject VFX_Hit;
    [HideInInspector] public GameObject VFX_Die;
    [SerializeField] private ParticleSystem _VFXTarget;
    private ParticleSystem _spawned;
    [SerializeField] private ParticleSystem guardBlocked;
    [SerializeField] private ParticleSystem shieldEffect;    
    [SerializeField] private ParticleSystem deathEffect;
    [SerializeField] private ParticleSystem _guardBreak;
    public static event Action<GameObject> OnDeath;

    [SerializeField] Transform hitOutput;
    private void Awake()
    {
        enemyData.SetUpEnemy(out maxHealth, out VFX_Hit, out VFX_Die, out guardHealth);
    }

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        barParent.localScale = Vector3.one;
        barSpriteRenderer.material.color = hpGradient.Evaluate(currentHealth / maxHealth);
        hpEnemy.SetActive(false);
    }

    private void Update()
    {
        if (CombatCamBehavior.Instance.closestTarget == transform)
        {
            if (_spawned != null)return;
            _spawned = Instantiate(_VFXTarget, transform.position + Vector3.up * 2, Quaternion.identity, transform);
        }
        else
        {
            Destroy(_spawned);
        }
    }

    public void TakeDamage(float damage)
    {
        if (isGuarding)
        {
            float tempoDmg = new();
            tempoDmg = Mathf.Clamp(damage / 2, 0, Mathf.Infinity);
            guardHealth -= tempoDmg;
            enemyBehaviour.GuardEnemySound();
            animator.Play("Block_hit");
            guardBlocked.Play();
            shieldEffect.Play();
            if (guardHealth <= 0)
            {
                GetStun();
            }
        }
        else
        {
            currentHealth -= damage;
            barSpriteRenderer.material.color = hpGradient.Evaluate(currentHealth / maxHealth);
            barParent.localScale = new Vector3((currentHealth / maxHealth), 1f);
            enemyBehaviour.GetHitEnemySound();
            if (currentHealth < maxHealth)
                hpEnemy.SetActive(true);
            if (currentHealth <= 0)
            {
                Die();
                Instantiate(VFX_Die,transform.position, Quaternion.identity);
            }

            Instantiate(VFX_Hit, hitOutput.position, Quaternion.identity);
        }
    }

    public void GetStun()
    {
        _guardBreak.Play();
        enemyBehaviour.ChangeState(EnemyState.Stun);
        isGuarding = false;
    }
    
    public void ResetGuard()
    {
        guardHealth = enemyData.GuardHealth;
    }
    
    public void Die()
    {
        enemyBehaviour.DeathEnemySound();
        Instantiate(deathEffect, hitOutput.position, Quaternion.identity);
        OnDeath?.Invoke(this.gameObject);
        Destroy(gameObject);
    }
}
