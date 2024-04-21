using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class CombatCamBehavior : MonoBehaviour
{
    /*[HideInInspector]*/ public List<Transform> targets;
    /*[HideInInspector]*/ public Transform closestTarget;
    /*[HideInInspector]*/ public Transform lockTarget;

    [Header("References")]
    public Transform playerTransform;
    public Transform orientation;
    
    //Private
    bool lockOn;

    private void OnEnable()
    {
        CharacterAnimatorEvents.OnLooktAtTarget += LookAtTarget;
    }
    private void OnDisable()
    {
        CharacterAnimatorEvents.OnLooktAtTarget -= LookAtTarget;
    }

    private void Update()
    {
        if (targets.Count >= 2)
        {
            targets = targets.Where(item => item != null).ToList();
            closestTarget = FindClosestEnemy(playerTransform.position, targets);
        }
        else if (targets.Count == 1)
        {
            closestTarget = targets.First();
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
}
