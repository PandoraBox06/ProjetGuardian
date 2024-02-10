using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHpBar : MonoBehaviour
{
    public Image fill;

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
        fill.fillAmount = PlayerData.Instance.currentHealth / PlayerData.Instance.maxHealth;
    }

    void UpdateHP(float hp)
    {
        fill.fillAmount = hp / PlayerData.Instance.maxHealth;
    }
}
