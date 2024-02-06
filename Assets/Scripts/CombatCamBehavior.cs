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
    public Transform playerBody;
    public Transform orientation;
    public CinemachineFreeLook combatCam;
    public CameraBehavior cameraBehavior;
    public InputActionReference targetAction;
    
    //Private
    bool lockOn;

    private void OnEnable()
    {
        targetAction.action.performed += TargetLock;
    }
    private void OnDisable()
    {
        targetAction.action.performed -= TargetLock;
    }

    private void Update()
    {
        if(lockTarget == null)
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

            if (closestTarget != null)
            {
                playerTransform.LookAt(closestTarget.position);
                playerBody.LookAt(closestTarget.position);
                orientation.LookAt(closestTarget.position);
            }

            combatCam.LookAt = playerTransform;
        }
        else if (lockTarget != null)
        {
            playerTransform.LookAt(lockTarget.position);
            playerBody.LookAt(lockTarget.position);
            orientation.LookAt(lockTarget.position);

            combatCam.LookAt = lockTarget;
        }

        if (closestTarget != null || lockTarget != null)
        {
            cameraBehavior.SwitchCameraStyle(CameraBehavior.CameraStyle.Combat);
        }
        else
            cameraBehavior.SwitchCameraStyle(CameraBehavior.CameraStyle.Basic);

    }

    void TargetLock(InputAction.CallbackContext callbackContext)
    {
        if (!lockOn)
        {
            lockTarget = closestTarget;
            lockOn = true;
        }
        else
        {
            lockTarget = null;
            lockOn = false;
        }
    }

    public Transform FindClosestEnemy(Vector3 playerPosition, List<Transform> enemies)
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
