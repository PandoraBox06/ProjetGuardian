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

    public static event Action<float> OnDamageTaken;

    private void Awake()
    {
        playerData = new(playerMaxHp);
        playerData.currentHealth = playerData.maxHealth;
    }
    public void TakeDamage(float damage)
    {
        // Debug.Log("Take Dmg"); //tu peux décommenter, j'en avait marre de voir cette ligne spawn ptdr
        playerData.currentHealth -= damage;
        OnDamageTaken?.Invoke(playerData.currentHealth);
        if (playerData.currentHealth <= 0)
            Die();
    }

    public void Die()
    {
        playerData.currentHealth = playerData.maxHealth;
    }

}
