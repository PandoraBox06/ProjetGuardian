using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SimpleAttackSystem : MonoBehaviour
{
    [SerializeField] private InputActionReference attackAction;
    [SerializeField] private InputActionReference rangeAction;
    [SerializeField] private InputActionReference dashAction;
    [SerializeField] private Animator _animator;
    [SerializeField] private string comboName = "Atk";
    [SerializeField] private string shootingName = "Shooting";
    [SerializeField] private int comboCount = 0;
    [SerializeField] private int maxComboCount = 0;

    [SerializeField] private float resetComboTimer = 1f;
    [SerializeField] private float timeBetweenAttack = .5f;
    private float timer;
    
    private void OnEnable()
    {
        attackAction.action.performed += Attack;
        rangeAction.action.performed += Range;
        dashAction.action.performed += Dash;
    }

    private void OnDisable()
    {
        attackAction.action.performed -= Attack;
        rangeAction.action.performed -= Range;
        dashAction.action.performed -= Dash;
    }

    void Attack(InputAction.CallbackContext context)
    {
        if (Time.time > timer)
        {
            CancelInvoke(nameof(ResetCombo));
            
            comboCount++;
            _animator.Play(comboName + comboCount);
            timer = Time.time + timeBetweenAttack;
            Invoke(nameof(ResetCombo), resetComboTimer);
            
            if (comboCount >= maxComboCount)
                ResetCombo();
                
        }
    }
    
    void Range(InputAction.CallbackContext context)
    {
        if (Time.time > timer)
        {
            _animator.Play(shootingName);
            timer = Time.time + timeBetweenAttack;
        }
    }

    void ResetCombo()
    {
        comboCount = 0;
    }

    void Dash(InputAction.CallbackContext context)
    {
        _animator.CrossFade("Exit", 0.2f);
    }
}
