using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerData
{
    public float maxHealth;
    public float currentHealth;

    public static PlayerData Instance;

    public PlayerData(float maxHealth)
    {
        this.maxHealth = maxHealth;
        this.currentHealth = maxHealth;
        Instance = this;
    }
}

public class PlayerStats : MonoBehaviour, IDamageable
{
    PlayerData playerData;

    public float playerMaxHp;

    [SerializeField] private Animator animator;
    private static readonly int Hit = Animator.StringToHash("Hit");
    private static readonly int Death = Animator.StringToHash("Death");

    [SerializeField] private ParticleSystem blood;
    [SerializeField] private ParticleSystem hit;
    private bool isDashing;
    private bool _iFrame;
    private bool _isDead;

    [SerializeField] private GameObject _lowHp;
    public static event Action<float> OnDamageTaken;
    public static event Action OnDeath;

    private void Awake()
    {
        playerData = new(playerMaxHp);
        playerData.currentHealth = playerData.maxHealth;
    }

    private void OnEnable()
    {
        DashController.OnDash += IsDashing;
        CharacterAnimatorEvents.OnIFrame += IsInvu;
        GameManager.OnFullRegen += FullRegen;
    }

    private void OnDisable()
    {
        DashController.OnDash -= IsDashing;
        CharacterAnimatorEvents.OnIFrame -= IsInvu;
        GameManager.OnFullRegen -= FullRegen;
    }

    public void TakeDamage(float damage)
    {
        if(_isDead) return;
        if (isDashing) return;
        if (_iFrame) return;
        playerData.currentHealth = Mathf.Clamp(playerData.currentHealth - damage, 0, playerData.maxHealth);
        _lowHp.SetActive(playerData.currentHealth <= playerData.maxHealth * 0.3);
        blood.Play();
        hit.Play();
        GetHitPlayerSound();
        OnDamageTaken?.Invoke(playerData.currentHealth);
        if (playerData.currentHealth <= 0)
            Die();
    }

    private void FullRegen()
    {
        playerData.currentHealth = playerData.maxHealth;
        _lowHp.SetActive(playerData.currentHealth <= playerData.maxHealth * 0.3);
        OnDamageTaken?.Invoke(playerData.currentHealth);
    }

    public void Die()
    {
        if(_isDead) return;
        OnDeath?.Invoke();
        DeathPlayerSound();
        animator.SetTrigger("Death");
        GetComponent<BlancoAnimationBehaviour>().enabled = false;
        GetComponent<BlancoCombatManager>().enabled = false;
        GetComponent<CameraBehavior>().enabled = false;
        GetComponent<PlayerMouvement>().enabled = false;
        GetComponent<DashController>().enabled = false;
  
        this.enabled = false;
        _isDead = true;
        // playerData.currentHealth = playerData.maxHealth;
    }

    public void GetStun()
    {
        
    }
    
    private void GetHitPlayerSound()
    {
        if (!AudioManager.Instance.getHit.IsNull)
            AudioManager.Instance.PlayOneShot(AudioManager.Instance.getHit, transform.position);
    }
    
    private void DeathPlayerSound()
    {
        if (!AudioManager.Instance.death.IsNull)
            AudioManager.Instance.PlayOneShot(AudioManager.Instance.death, transform.position);
    }
    
    private void IsDashing(bool b)
    {
        isDashing = b;
    }
    private void IsInvu(bool b)
    {
        _iFrame = b;
    }

}
