using System;
using UnityEngine;


public class GuardBreak : MonoBehaviour
{
    [Header("GuardBreaking")]
    [SerializeField] private float _weaponRadius;
    [SerializeField] private float _weaponDamage;
    [SerializeField] private LayerMask _enemyMask;
    [SerializeField] private Transform _weaponTransform;

    private void Start()
    {
        _weaponDamage = GetComponentInParent<Weapon>().damage;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Enemy")) return;
        other.TryGetComponent(out IDamageable damageable);
        if (damageable == null) return;
        damageable.GetStun();
        damageable.TakeDamage(_weaponDamage);
    }
}
