using System;
using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class CombatCamBehavior : MonoBehaviour
{
    public static CombatCamBehavior Instance;
    [HideInInspector] public List<Transform> targets;
     public Transform closestTarget;

    [Header("References")]
    public Transform playerTransform;
    public Transform orientation;
    //Private
    bool lockOn;

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        // CharacterAnimatorEvents.OnLooktAtTarget += LookAtTarget;
        BlancoCombatManager.LookTarget += LookAtTarget;
    }
    private void OnDisable()
    {
        // CharacterAnimatorEvents.OnLooktAtTarget -= LookAtTarget;
        BlancoCombatManager.LookTarget -= LookAtTarget;

    }

    private void Update()
    {
        switch (targets.Count)
        {
            case >= 2:
                targets = targets.Where(item => item != null).ToList();
                closestTarget = FindClosestEnemy(playerTransform.position, targets);
                break;
            case 1:
                closestTarget = targets.First();
                break;
        }
    }
    
    private void LookAtTarget()
    {
        if (closestTarget != null)
        {
            //Debug.Log("Look at this");
            playerTransform.LookAt(closestTarget.position);
            //orientation.LookAt(closestTarget.position);
        }
    }

    private Transform FindClosestEnemy(Vector3 playerPosition, List<Transform> enemies)
    {
        Transform closestEnemy = null;
        float closestDistance = Mathf.Infinity;
        targets = targets.Where(item => item != null).ToList();

        foreach (Transform enemy in enemies)
        {
            float distance = Vector3.Distance(playerPosition, enemy.transform.position);

            if (distance < closestDistance)
            {
                closestEnemy = enemy;
                closestDistance = distance;
            }
        }
        return closestEnemy;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            targets.Add(other.transform);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            targets.Remove(other.transform);
            if(targets.Count == 0 )
            {
                closestTarget = null;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        SphereCollider col = GetComponent<SphereCollider>();
        Gizmos.DrawWireSphere(transform.position, col.radius);
    }
}
