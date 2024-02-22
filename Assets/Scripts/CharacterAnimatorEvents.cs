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
    public static event Action OnParseInputOn;
    public static event Action OnParseInputOff;
    public static event Action OnPauseToParse;
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

    public void ParseInputOn()
    {
        OnParseInputOn?.Invoke();
    }
    public void ParseInputOff()
    {
        OnParseInputOff?.Invoke();
    }

    public void AddPauseToParse()
    {
        OnPauseToParse?.Invoke();
    }
    
    public void StepEvent()
    {
        RuntimeManager.PlayOneShot(stepAudio);
    }
}
