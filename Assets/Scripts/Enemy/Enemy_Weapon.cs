using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Weapon : MonoBehaviour
{
    [SerializeField] private Enemy_Data enemyData;
    private string weaponName;
    private float damage;
    public Collider weaponCol;

    public WeaponType weaponType;
    public enum WeaponType
    {
        melee,
        ranged
    }
    private void Awake()
    {
        enemyData.SetUpWeapon(damage);
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

    private void OnDisbaleCollider(GameObject go)
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
