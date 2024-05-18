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

    public static event Action<float> OnDamageTaken;

    private void Awake()
    {
        playerData = new(playerMaxHp);
        playerData.currentHealth = playerData.maxHealth;
    }

    private void OnEnable()
    {
        DashController.OnDash += IsDashing;
    }

    private void OnDisable()
    {
        DashController.OnDash -= IsDashing;
    }

    public void TakeDamage(float damage)
    {
        if (isDashing) return;
        animator.SetTrigger(Hit);
        playerData.currentHealth -= damage;
        blood.Play();
        hit.Play();
        GetHitPlayerSound();
        OnDamageTaken?.Invoke(playerData.currentHealth);
        if (playerData.currentHealth <= 0)
            Die();
    }

    public void Die()
    {
        DeathPlayerSound();
        animator.SetTrigger(Death);
        GetComponent<BlancoCombatManager>().enabled = false;
        GetComponent<CameraBehavior>().enabled = false;
        GetComponent<PlayerMouvement>().enabled = false;
        GetComponent<DashController>().enabled = false;
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

}
