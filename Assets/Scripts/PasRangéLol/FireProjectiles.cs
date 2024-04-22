using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FireProjectiles : MonoBehaviour
{
    [SerializeField] private Transform _projectileOutput;
    [SerializeField] private float _projectileSpeed;
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private CombatCamBehavior _combatCamBehavior;
    [SerializeField] private Transform _projectileDump;
    private Transform target;

    private void OnEnable()
    {
        CharacterAnimatorEvents.OnFireProjectile += FireProjectile;
    }

    private void OnDisable()
    {
        CharacterAnimatorEvents.OnFireProjectile -= FireProjectile;
    }

    private void Start()
    {
        _combatCamBehavior = GetComponentInChildren<CombatCamBehavior>();
    }

    private void Update()
    {
        if (_combatCamBehavior.closestTarget == null) return;
        target = _combatCamBehavior.closestTarget;
    }

    private void FireProjectile()
    {
        if (target != null)
        {
            Vector3 projectileOutputPosition = _projectileOutput.position;
            GameObject projectile = Instantiate(_projectilePrefab, projectileOutputPosition, Quaternion.identity, _projectileDump);
            Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();
            Vector3 direction =  target.position - projectileOutputPosition;
            direction += Vector3.up;
            projectileRb.AddForce(direction * _projectileSpeed, ForceMode.Force);
        }
        else
        {
            GameObject projectile = Instantiate(_projectilePrefab,  _projectileOutput.position, Quaternion.identity, _projectileDump);
            Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();
            Vector3 direction =  _projectileOutput.forward;
            projectileRb.AddForce(direction * _projectileSpeed, ForceMode.Force);
        }
    }
}
