using System;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine;

public class Fuckall : MonoBehaviour
{
    [SerializeField] private List<InputAction> currentCombo = new List<InputAction>();
    
    [SerializeField] private InputActionReference attackInput;
    [SerializeField] private InputActionReference rangeInput;

    [SerializeField] private BlancoAnimationBehaviour animManager;

    private void Start()
    {
        attackInput.action.performed += StartInput;
        rangeInput.action.performed += StartInput;
        
        attackInput.action.canceled += EndInput;
    }

    private void StartInput(InputAction.CallbackContext callback)
    {
        // GameManager.Instance
        if (callback.action == rangeInput.action)
        {
            currentCombo.Add(callback.action);
            TryRange();
        }
        else if (callback.action == attackInput.action)
        {
            currentCombo.Add(callback.action);
            TryAttack();
        }
    }

    private void EndInput(InputAction.CallbackContext callback)
    {
        
    }
    
    private void TryAttack()
    {
        if (currentCombo.Count == 1)
        {
            animManager.TryToPlay("Attack_1");
        }
        else if (currentCombo.Count <= 5)
        {
            animManager.FollowWithInput(ActionType.Attack);
        }
        else RestartCombo();
    }

    private void TryHold()
    {
        
    }

    private void TryRange()
    {
        if (currentCombo.Count == 1)
        {
            animManager.TryToPlay("Range_1");
        }
        RestartCombo();
    }

    private void TryPause()
    {
        
    }
    
    private void RestartCombo()
    {
        currentCombo = new List<InputAction>();
        // CancelEvent?.Invoke();
    }
}
