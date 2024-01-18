using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    public float maxHealth;
    public float currentHealth;
    public Gradient hpGradient;
    Renderer _renderer;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        _renderer = GetComponentInChildren<Renderer>();
        _renderer.material.color = hpGradient.Evaluate(currentHealth / maxHealth);
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        _renderer.material.color = hpGradient.Evaluate(currentHealth / maxHealth);
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}
