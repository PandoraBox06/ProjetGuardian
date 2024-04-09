using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHpBar : MonoBehaviour
{
    public Slider slider;

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
    }

    void UpdateHP(float hp)
    {
        slider.value = hp;
    }
}
