using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine.InputSystem;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Fuckall : MonoBehaviour
{
    #region Variables
    [SerializeField] private List<InputAction> currentCombo = new List<InputAction>();
    
    [SerializeField] private InputActionReference attackInput;
    [SerializeField] private InputActionReference rangeInput;

    [SerializeField] private BlancoAnimationBehaviour animManager;
    
    public Slider inputTimingSlider;

    public string lastFinishedCombo { get; private set; }
    
    [HideInInspector] public UnityEvent InputEvent;
    [HideInInspector] public UnityEvent CancelEvent;
    [HideInInspector] public UnityEvent FinishedComboEvent;
    public InputAction actionInput { get; private set; }

    public bool CanInput = true;
    
    public static Fuckall Instance
    {
        get
        {
            if (_instance == null) _instance = FindObjectOfType<Fuckall>();
            return _instance;
        }
    }
    private static Fuckall _instance;
    #endregion
    
    private void Start()
    {
        attackInput.action.performed += StartInput;
        rangeInput.action.performed += StartInput;
        
        attackInput.action.canceled += EndInput;
        
        inputTimingSlider.gameObject.SetActive(false);

        CanInput = true;
    }

    private void StartInput(InputAction.CallbackContext callback)
    {
        NewInput(callback.action);
    }

    private void EndInput(InputAction.CallbackContext callback)
    {
        
    }

    private void NewInput(InputAction input, bool isRetry = false)
    {
        Debug.Log("new input "+input);
        if (!GameManager.Instance.IsInPlayMode() || !CanInput) return;
        
        if (input == rangeInput.action)
        {
            currentCombo.Add(input);
            TryRange();
        }
        else if (input == attackInput.action)
        {
            currentCombo.Add(input);
            TryAttack();
        }
        else RestartCombo();
    }

    #region TryInput
    private void TryAttack(bool isRetry = false)
    {
        Debug.Log("Attack "+isRetry);
        if (currentCombo.Count == 1)
        {
            animManager.TryToPlay("Attack_1");
            ShowInput(attackInput.action);
        }
        else if (currentCombo.Count <= 5)
        {
            animManager.FollowWithInput(ActionType.Attack);
            ShowInput(attackInput.action);
        }
        else
        {
            if (!isRetry) RetryCombo(attackInput);
        }
    }

    private void TryHold(bool isRetry = false)
    {
        
    }

    private void TryRange(bool isRetry = false)
    {
        Debug.Log("Range "+isRetry);
        if (currentCombo.Count == 1)
        {
            animManager.TryToPlay("Range_1");
            ShowInput(rangeInput.action, true);
        }
        if (!isRetry) RetryCombo(rangeInput);
    }

    private void TryPause(bool isRetry = false)
    {
        
    }
    #endregion

    //For when the player is mistaken and we want to retry their input
    private void RetryCombo(InputAction input)
    {
        // Debug.Log("*** retry ***");
        // RestartCombo();
        // NewInput(input, true);
    }

    public void FinishCombo(string comboName)
    {
        lastFinishedCombo = comboName;
        FinishedComboEvent?.Invoke();
        Debug.Log($"{comboName} has been finished");
        RestartCombo();
    }
    
    public void RestartCombo()
    {
        Debug.Log("*** restart ***");
        currentCombo = new List<InputAction>();
        CanInput = true;
        CancelEvent?.Invoke();
    }
    
    private void ShowInput(InputAction input, bool isFinal = false)
    {
        actionInput = input;
        InputEvent?.Invoke();
        if (isFinal)
        {
            FinishedComboEvent?.Invoke();
        }
    }
}
