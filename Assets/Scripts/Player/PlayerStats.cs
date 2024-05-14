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
    
    public static event Action<float> OnDamageTaken;

    private void Awake()
    {
        playerData = new(playerMaxHp);
        playerData.currentHealth = playerData.maxHealth;
    }
    public void TakeDamage(float damage)
    {
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
        playerData.currentHealth = playerData.maxHealth;
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

}
