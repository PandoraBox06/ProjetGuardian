using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    //public
    [HideInInspector] public float currentHealth;
    
    //private
    [SerializeField] private SpriteRenderer barSpriteRenderer;
    [SerializeField] private Transform barParent;
    [SerializeField] private float maxHealth;
    [SerializeField] private Gradient hpGradient;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        barParent.localScale = Vector3.one;
        barSpriteRenderer.material.color = hpGradient.Evaluate(currentHealth / maxHealth);
    }

    public void TakeDamage(float damage)
    {
        barParent.localScale = new Vector3((currentHealth / maxHealth), 1f);
        currentHealth -= damage;
        barSpriteRenderer.material.color = hpGradient.Evaluate(currentHealth / maxHealth);
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
