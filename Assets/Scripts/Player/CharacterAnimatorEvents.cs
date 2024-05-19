using System;
using UnityEngine;
using UnityEngine.Events;
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
    [SerializeField] private Animator animator;
    [Header("Attack Slide (combo2)")]
    private Vector3 _slideDirection;
    [SerializeField] private float _slideDistance = 20f;
    [SerializeField] private float _slideDuration = 0.4f;
    private CharacterController _characterController;
    [Header("VFX")] 
    [SerializeField] private ParticleSystem holdVFX;
    [SerializeField] private Transform vfxOutput;
    [Header("GuardBreaking")]
    [SerializeField]  private Collider _guardBreak;
    [SerializeField] private ParticleSystem _guardBreakerVfx;

    public static event Action<bool> OnIFrame;
    private bool _iframe;
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
    
    public void HitCrossbowSound()
    {
        if (!AudioManager.Instance.hitCrossbow.IsNull)
            AudioManager.Instance.PlayOneShot(AudioManager.Instance.hitCrossbow, transform.position);
    }

    public void IFramee()
    {
        if (!_iframe)
        {
            _iframe = true;
            OnIFrame?.Invoke(_iframe);
        }
        else
        {
            _iframe = false;
            OnIFrame?.Invoke(_iframe);
        }
    }

    public void TurnOffDash()
    {
        _guardBreak.enabled = false;
        _iframe = false;
        OnIFrame?.Invoke(_iframe);
        OnDisbaleColliderCall?.Invoke();
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

    public void HoldGood()
    {
        Instantiate(holdVFX, vfxOutput.position, Quaternion.identity);
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
        _slideDirection = transform.forward;

        // Ignore Y component to avoid dashing upwards
        _slideDirection.y = 0;

        // Normalize dash direction to avoid faster dashes when looking up or down
        _slideDirection.Normalize();
        
        // Move the player in dash direction
        animator.applyRootMotion = false;
        _characterController.Move(_slideDirection * ((_slideDistance / _slideDuration) * Time.deltaTime));
        animator.applyRootMotion = true;
    }

    public void GuardBreak()
    {
        _guardBreak.enabled = true;
        _guardBreakerVfx.Play();
    }

    public void GuardBreakOff()
    {
        _guardBreak.enabled = false;
    }
}
