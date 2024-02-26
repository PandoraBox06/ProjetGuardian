using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;

public class CharacterAnimatorEvents : MonoBehaviour
{
    
    [SerializeField] private EventReference stepAudio;
    public static event Action OnEnableColliderCall;
    public static event Action OnDisbaleColliderCall;
    public static event Action OnFireProjectile;
    public static event Action OnLooktAtTarget;
    [SerializeField] PlayerControlerV2 playerControler;
    [SerializeField] private Collider playerCollider;
    public void OnEnableCollider()
    {
        OnEnableColliderCall?.Invoke();
    }
    public void OnDisableCollider()
    {
        OnDisbaleColliderCall?.Invoke();
    }

    public void OnFireProjectiles()
    {
        OnFireProjectile?.Invoke();
    }

    public void OnLookAtTargets()
    {
        OnLooktAtTarget?.Invoke();
    }
    
    public void StepEvent()
    {
        RuntimeManager.PlayOneShot(stepAudio);
    }

    public void PlayerIFrameOn()
    {
        playerCollider.enabled = false;
    }

    public void PlayerIFrameOff()
    {
        playerCollider.enabled = true;
    }

    public void FreezeMovement()
    {
        playerControler.isAttacking = true;
    }
    public void UnFreezeMovement()
    {
        playerControler.isAttacking = false;
    }
}
