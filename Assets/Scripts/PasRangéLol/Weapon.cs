using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public string weaponName;
    public float damage;
    public Collider weaponCol;

    public WeaponType weaponType;
    public enum WeaponType
    {
        melee,
        ranged
    }

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
        if(weaponType == WeaponType.melee)
            weaponCol.enabled = true;
    }

    public void OnDisbaleCollider()
    {
        if (weaponType == WeaponType.melee)
            weaponCol.enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.TryGetComponent(out IDamageable damageable);
            damageable.TakeDamage(damage);
            if (weaponType == WeaponType.ranged)
                Destroy(gameObject);
        }
    }
}
