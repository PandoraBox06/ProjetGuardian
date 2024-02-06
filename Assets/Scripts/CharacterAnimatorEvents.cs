using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimatorEvents : MonoBehaviour
{
    public static event Action OnEnableColliderCall;
    public static event Action OnDisbaleColliderCall;
    public static event Action OnFireProjectile;
    public static event Action OnLooktAtTarget;
    public void OnEnableeCollider()
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
}
