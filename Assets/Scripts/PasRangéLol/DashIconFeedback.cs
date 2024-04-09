using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DashIconFeedback : MonoBehaviour
{
    public Image fill;

    public float dashCD;
    float timer;

    private void OnEnable()
    {
        Dashing.OnDash += Dash;
    }

    private void OnDisable()
    {
        Dashing.OnDash -= Dash;
    }

    private void Start()
    {
        timer = dashCD;
    }

    void Update()
    {
        if (timer < dashCD)
        {
            timer += Time.deltaTime;
        }

        fill.fillAmount = timer / dashCD;
    }

    void Dash()
    {
        timer = 0;
    }
}
