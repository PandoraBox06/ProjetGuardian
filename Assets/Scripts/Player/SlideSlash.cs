using UnityEngine;


public class SlideSlash : MonoBehaviour
{
    [Header("Great Slash")]
    [SerializeField] private float _weaponRadius;
    [SerializeField] private float _weaponDamage;
    [SerializeField] private LayerMask _enemyMask;
    [SerializeField] private Transform _weaponTransform;

    private void Start()
    {
        _weaponDamage = GetComponentInParent<Weapon>().damage;
        _weaponDamage *= 3;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Enemy")) return;
        other.TryGetComponent(out IDamageable damageable);
        if (damageable == null) return;
        damageable.TakeDamage(_weaponDamage);
    }
}
