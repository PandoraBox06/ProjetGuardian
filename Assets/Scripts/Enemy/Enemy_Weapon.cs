using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Weapon : MonoBehaviour
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
        Enemy_AnimatorEvents.OnEnableColliderCall += OnEnableCollider;
        Enemy_AnimatorEvents.OnDisbaleColliderCall += OnDisbaleCollider;
    }
    private void OnDisable()
    {
        Enemy_AnimatorEvents.OnEnableColliderCall -= OnEnableCollider;
        Enemy_AnimatorEvents.OnDisbaleColliderCall -= OnDisbaleCollider;
    }

    public void OnEnableCollider(GameObject go)
    {
        if(go == this.transform.root.gameObject)
        {
            if (weaponType == WeaponType.melee)
                weaponCol.enabled = true;
        }
    }

    public void OnDisbaleCollider(GameObject go)
    {
        if(go == this.transform.root.gameObject)
        {
            if (weaponType == WeaponType.melee)
                weaponCol.enabled = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.TryGetComponent(out IDamageable damageable);
            damageable.TakeDamage(damage);
            if (weaponType == WeaponType.ranged)
                Destroy(gameObject);
        }
    }
}
