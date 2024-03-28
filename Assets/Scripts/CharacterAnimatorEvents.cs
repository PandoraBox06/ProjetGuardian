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
    public static event Action OnEndAnimation;
    public static event Action OnFireProjectile;
    public static event Action OnLooktAtTarget;
    
    [SerializeField] PlayerControlerV2 playerControler;
    [SerializeField] private CameraBehavior _cameraBehavior;
    [SerializeField] private Collider playerCollider;
    [SerializeField] private Animator animator;
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
        _cameraBehavior.isAttacking = true;
    }
    public void UnFreezeMovement()
    {
        playerControler.isAttacking = false;
        // _cameraBehavior.isAttacking = false;
    }

    public void OnEndAnimations()
    {
        OnEndAnimation?.Invoke();
    }

    public void ApplyRootMotionBack()
    {
        animator.applyRootMotion = !animator.applyRootMotion;
    }
    
    public void ClearRootMotionBack()
    {
        animator.applyRootMotion = false;
    }
}
