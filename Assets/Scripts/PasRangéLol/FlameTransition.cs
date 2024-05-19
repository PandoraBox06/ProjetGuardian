using System;
using System.Collections.Generic;
using UnityEngine;


public class FlameTransition : MonoBehaviour
{
    [SerializeField] private List<Animator> _flames;
    [SerializeField] private Transform _light1;
    [SerializeField] private Transform _light2;

    private void Awake()
    {
        for (int i = 0; i <  _light1.childCount; i++)
        {
            _flames.Add(_light1.GetChild(i).GetComponentInChildren<Animator>());
        }
        for (int i = 0; i <  _light2.childCount; i++)
        {
            _flames.Add(_light2.GetChild(i).GetComponentInChildren<Animator>());    
        }
    }

    private void OnEnable()
    {
        WaveManager.OnWaveEnd += FlameFlickering;
    }

    private void OnDisable()
    {
        WaveManager.OnWaveEnd -= FlameFlickering;
    }

    private void FlameFlickering()
    {
        foreach (var flame in _flames)
        {
            flame.Play("Light_Flickering");
        }
    }

    private void FlameOff()
    {
        foreach (var flame in _flames)
        {
            flame.gameObject.SetActive(false);
        }
    }

    private void FlameOn()
    {
        foreach (var flame in _flames)
        {
            flame.gameObject.SetActive(true);
        }
    }
}
