using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public string weaponName;
    public float damage;
    public Collider weaponCol;

    private void OnEnable()
    {
        CharacterAnimatorEvents.OnEnableColliderCall += OnEnableCollider;
        CharacterAnimatorEvents.OnDisbaleColliderCall += OnDisbaleCollider;

    }
    private void OnDisable()
    {
        CharacterAnimatorEvents.OnEnableColliderCall -= OnEnableCollider;
        CharacterAnimatorEvents.OnDisbaleColliderCall -= OnDisbaleCollider;

    }

    public void OnEnableCollider()
    {
        weaponCol.enabled = true;
        Debug.Log("call");
    }

    public void OnDisbaleCollider()
    {
        weaponCol.enabled = false;
        Debug.Log("is call");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.TryGetComponent(out IDamageable damageable);
            damageable.TakeDamage(damage);
        }
    }
}
