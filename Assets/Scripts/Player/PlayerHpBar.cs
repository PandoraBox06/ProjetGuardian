using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHpBar : MonoBehaviour
{
    public Slider slider;
    [SerializeField] private RectTransform fill;
    [SerializeField] private RectTransform animFill;

    private void OnEnable()
    {
        PlayerStats.OnDamageTaken += UpdateHP;
    }
    private void OnDisable()
    {
        PlayerStats.OnDamageTaken -= UpdateHP;
    }

    private void Start()
    {
        slider.maxValue = PlayerData.Instance.maxHealth;
        slider.value = PlayerData.Instance.currentHealth;
        animFill.anchorMax = fill.anchorMax;
    }

    void UpdateHP(float hp)
    {
        slider.value = hp;
        animFill.DOAnchorMax(fill.anchorMax, 1f);
    }
    
    private void OnDestroy()
    {
        animFill.DOKill();
    }
}