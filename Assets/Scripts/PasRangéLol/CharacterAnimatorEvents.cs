using System;
using System.Collections;
using System.Collections.Generic;
using FMODUnity;
using UnityEngine;
using UnityEngine.Serialization;

public class CharacterAnimatorEvents : MonoBehaviour
{
    public static event Action OnEnableColliderCall;
    public static event Action OnDisbaleColliderCall;
    public static event Action OnEndAnimation;
    public static event Action OnFireProjectile;
    public static event Action OnLooktAtTarget;
    
    [SerializeField] PlayerMouvement playerController;
    [SerializeField] private CameraBehavior _cameraBehavior;
    [SerializeField] private Collider playerCollider;
    [SerializeField] private Animator animator;
    
    private Vector3 dashDirection;
    [SerializeField] private float dashDistance = 5f;
    [SerializeField] private float dashDuration = 0.2f;
    private CharacterController _characterController;

    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
    }

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
        if (!AudioManager.Instance.walk.IsNull)
            AudioManager.Instance.PlayOneShot(AudioManager.Instance.walk, transform.position);
    }

    public void DoHitPlayerSound()
    {
        if (!AudioManager.Instance.doHit.IsNull)
            AudioManager.Instance.PlayOneShot(AudioManager.Instance.doHit, transform.position);
    }

    public void HitSwordSound()
    {
        if (!AudioManager.Instance.hitSword.IsNull)
            AudioManager.Instance.PlayOneShot(AudioManager.Instance.hitSword, transform.position);
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
        playerController.isAttacking = true;
        _cameraBehavior.isAttacking = true;
    }
    public void UnFreezeMovement()
    {
        playerController.isAttacking = false;
        _cameraBehavior.isAttacking = false;
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

    public void MoveImpulse()
    {
        // Get camera forward direction as dash direction
        dashDirection = transform.forward;

        // Ignore Y component to avoid dashing upwards
        dashDirection.y = 0;

        // Normalize dash direction to avoid faster dashes when looking up or down
        dashDirection.Normalize();
        
        // Move the player in dash direction
        animator.applyRootMotion = false;
        _characterController.Move(dashDirection * ((dashDistance / dashDuration) * Time.deltaTime));
        animator.applyRootMotion = true;
    }
    
    // private void ResetAllAnimatorTriggers()
    // {
    //     foreach (var trigger in animator.parameters)
    //     {
    //         if (trigger.type == AnimatorControllerParameterType.Trigger)
    //         {
    //             animator.ResetTrigger(trigger.name);
    //         }
    //     }
    // }
}
