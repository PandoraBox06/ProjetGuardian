using System;
using UnityEngine;


public class ChangeToGray : MonoBehaviour
{
    private GrayscalePostProcess _grayscale;

    private void Awake()
    {
        _grayscale = GetComponent<GrayscalePostProcess>();
    }

    private void OnEnable()
    {
        PlayerStats.OnDeath += ChangeToGrayscale;
    }

    private void OnDisable()
    {
        PlayerStats.OnDeath -= ChangeToGrayscale;
    }

    private void ChangeToGrayscale()
    {
        _grayscale.enabled = true;
    }
}
