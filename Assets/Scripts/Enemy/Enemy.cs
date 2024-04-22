using System;
using BasicEnemyStateMachine;
using System.Collections;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

public class Enemy : MonoBehaviour, IDamageable
{
    //public
    [SerializeField] private Enemy_Data enemyData;
    [HideInInspector] public float currentHealth;
    public float guardHealth;
    [SerializeField] EnemyBehaviour enemyBehaviour;
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
    [HideInInspector] public bool HasInstanciated;

    [SerializeField] private ParticleSystem guardBlocked;
    [SerializeField] private ParticleSystem shieldEffect;
    public static event Action<GameObject> OnDeath;

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

        if (VFX_Hit != null)
        {
            VFX_Hit.SetActive(false);
        }
        HasInstanciated = false;
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
                enemyBehaviour.ChangeState(Enemy_State.Stun);
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
                HasInstanciated = true;
            }

            Instantiate(VFX_Hit, transform.position + Vector3.up, Quaternion.identity);
        }
    }

    public void ResetGuard()
    {
        guardHealth = enemyData.GuardHealth;
    }
    
    public void Die()
    {
        enemyBehaviour.DeathEnemySound();
        Destroy(gameObject);
        OnDeath?.Invoke(this.gameObject);
    }
}
