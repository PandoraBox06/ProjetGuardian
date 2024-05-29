using System;
using UnityEngine;


public class PlayQuickSound : MonoBehaviour
{
    private void Start()
    {
        if(!AudioManager.Instance.attackRangeEnemyImpact.IsNull)
            AudioManager.Instance.PlayOneShot(AudioManager.Instance.attackRangeEnemyImpact, transform.position);
    }
}
