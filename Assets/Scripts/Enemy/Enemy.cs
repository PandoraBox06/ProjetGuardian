using BasicEnemyStateMachine;
using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    //public
    [HideInInspector] public float currentHealth;
    [SerializeField] BaseEnemy_StateManager stateManager;
    //private
    [SerializeField] private GameObject hpEnemy;
    [SerializeField] private SpriteRenderer barSpriteRenderer;
    [SerializeField] private Transform barParent;
    public float maxHealth;
    [SerializeField] private Gradient hpGradient;

    [Header("VFX")]
    public GameObject VFX_Hit;
    public GameObject VFX_Die;
    public bool HasInstanciated;
    
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

    void VFXHit()
    {
        VFX_Hit.SetActive(true);
        // Debug.Log("Oui");

    }

    IEnumerator DeactivateVFXHit()
    {

        yield return new WaitForSeconds(1f);

        VFX_Hit.SetActive(false);
    }

    IEnumerator DestroyExplosion()
    {

        yield return new WaitForSeconds(1f);

        VFX_Die.SetActive(false);
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        barSpriteRenderer.material.color = hpGradient.Evaluate(currentHealth / maxHealth);
        barParent.localScale = new Vector3((currentHealth / maxHealth), 1f);
        if(stateManager.combatType == BaseEnemy_StateManager.CombatMode.dodge)
            stateManager.isStunned = true;
        if (currentHealth < maxHealth)
            hpEnemy.SetActive(true);
        if (currentHealth <= 0)
        {
            Die();
            Instantiate(VFX_Die,transform.position, Quaternion.identity);
            HasInstanciated = true;
        }

        if (HasInstanciated)
        {
            StartCoroutine(DestroyExplosion());
        }

        Invoke(nameof(VFXHit), 0);
        StartCoroutine(DeactivateVFXHit());
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}
