using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    public Transform healthBar;
    public float maxHealth;
    [HideInInspector] public float currentHealth;
    public Gradient hpGradient;
    Renderer _renderer;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        healthBar.localScale = Vector3.one;
        _renderer = GetComponentInChildren<Renderer>();
        _renderer.material.color = hpGradient.Evaluate(currentHealth / maxHealth);
    }

    public void TakeDamage(float damage)
    {
        healthBar.localScale = new Vector3((currentHealth / maxHealth), 1f);
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
