using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationsDurations : MonoBehaviour
{
    public static AnimationsDurations Instance { get; private set; }
    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    private void OnDestroy()
    {
        if (this == Instance)
            Instance = null;
    }

    private float lengthOfCurrentAnim;
    
    public void UpdateAnimInfo(float animLength)
    {
        lengthOfCurrentAnim = animLength;
    }
    
    public float GetDuration()
    {
        return lengthOfCurrentAnim;
    }
}