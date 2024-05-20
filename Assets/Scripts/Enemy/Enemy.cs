using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour, IDamageable
{
    //public
    [SerializeField] private Enemy_Data enemyData;
    [HideInInspector] public float currentHealth;
    public float guardHealth;
    [SerializeField] NewEnemyBehaviour enemyBehaviour;
    [SerializeField] private Animator animator;
    //private
    [HideInInspector] public float maxHealth;
    [SerializeField] private Gradient hpGradient;
    public bool isGuarding;
    
    [Header("VFX")]
    [HideInInspector] public GameObject VFX_Hit;
    [SerializeField] private ParticleSystem _VFXTarget;
    private ParticleSystem _spawned;
    [SerializeField] private ParticleSystem guardBlocked;
    [SerializeField] private ParticleSystem shieldEffect;    
    [SerializeField] private ParticleSystem deathEffect;
    [SerializeField] private ParticleSystem _guardBreak;
    public static event Action<GameObject> OnDeath;

    [Header("Interface")]
    [SerializeField] private Canvas enemyCanvas;
    [SerializeField] private Slider slider;
    [SerializeField] private RectTransform fill;
    [SerializeField] private RectTransform animFill;

    [SerializeField] Transform hitOutput;
    private void Awake()
    {
        enemyData.SetUpEnemy(out maxHealth, out VFX_Hit, out guardHealth);
    }

    // Start is called before the first frame update
    void Start()
    {
        enemyCanvas.worldCamera = Camera.main;
        currentHealth = maxHealth;
        slider.maxValue = maxHealth;
        slider.value = currentHealth;
        animFill.anchorMax = fill.anchorMax;
        enemyCanvas.gameObject.SetActive(false);
    }

    private void Update()
    {
        //To make sure the hpBar is looking at the camera
        enemyCanvas.transform.LookAt(enemyCanvas.worldCamera.transform);
        
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
            slider.value = currentHealth;
            if (animFill != null && fill != null) animFill.DOAnchorMax(fill.anchorMax, 1f);
            enemyBehaviour.GetHitEnemySound();
            if (currentHealth < maxHealth)
                enemyCanvas.gameObject.SetActive(true);
            if (currentHealth <= 0)
            {
                Die();
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

    private void OnDestroy()
    {
        animFill.DOKill();
    }
}
