using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_AnimatorEvents : MonoBehaviour
{
    public static event Action<GameObject> OnEnableColliderCall;
    public static event Action<GameObject> OnDisbaleColliderCall;

    public void OnEnableeCollider()
    {
        OnEnableColliderCall?.Invoke(this.transform.root.gameObject);
    }
    public void OnDisableCollider()
    {
        OnDisbaleColliderCall?.Invoke(this.transform.root.gameObject);
    }
}
