using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class BlancoCombatManager : MonoBehaviour
{
    #region Variables
    //-----instance-------------------------------------------------------
    public static BlancoCombatManager Instance
    {
        get
        {
            if (_instance == null) _instance = FindObjectOfType<BlancoCombatManager>();
            return _instance;
        }
    }
    private static BlancoCombatManager _instance;
    //-----public---------------------------------------------------------
    [HideInInspector] public UnityEvent InputEvent;
    [HideInInspector] public UnityEvent CancelEvent;
    [HideInInspector] public UnityEvent FinishedComboEvent;
    public InputAction actionInput { get; private set; }
    public static event Action LookTarget;
    public ComboScriptableObject finishedCombo { get; private set; }
    //-----sérialisé------------------------------------------------------
    [SerializeField] private Animator animator;
    
    [Header("Inputs")]
    public InputActionReference pauseInput;
    public InputActionReference attackInput;
    public InputActionReference gunInput;
    public InputActionReference holdInput;
    private InputAction NextInputContainer;
    private InputAction NoneInputContainer;
    private InputAction HoldInputContainer;
    
    [Header("Settings")]
    [SerializeField] private float transitionDuration;
    [SerializeField] private float holdMinDuration;
    [SerializeField] private List<ComboScriptableObject> allCombos = new List<ComboScriptableObject>();
    //-----privé------------------------------------------------------
    private List<ComboScriptableObject> validList = new List<ComboScriptableObject>();
    private List<InputAction> currentCombo = new List<InputAction>();
    
    private float elapsedTime;
    private float holdTime;
    
    private bool isHoldPossible;
    private bool isHoldFinished;
    private bool canChainInput;

    public string currentAnimationName;
    
    private const string INPUT_NONE = "None_Input";
    #endregion
    
    private void OnEnable()
    {
        CharacterAnimatorEvents.OnEndAnimation += FinishedAnimation;
    }

    private void OnDisable()
    {
        CharacterAnimatorEvents.OnEndAnimation -= FinishedAnimation;
        //detect all inputs in scriptableObjects
        List<InputAction> _allInputs = new List<InputAction>();
        foreach (ComboScriptableObject combo in allCombos)
        {
            foreach (var t in combo.inputList.Where(t => !_allInputs.Contains(t.action)))
            {
                _allInputs.Add(t.action);
            }
        }
        foreach (InputAction input in _allInputs)
        {
            input.performed -= StartInput;
            input.canceled -= CancelInput;
        }
    }
    private void Start()
    {
        NoneInputContainer = new InputAction(INPUT_NONE);
        NextInputContainer = NoneInputContainer;
        
        elapsedTime = transitionDuration +1;
        //detect all inputs in scriptableObjects
        List<InputAction> _allInputs = new List<InputAction>();
        foreach (ComboScriptableObject combo in allCombos)
        {
            for (int i = 0; i < combo.inputList.Count; i++)
            {
                // Register to attacks
                if (!_allInputs.Contains(combo.inputList[i].action))
                {
                    _allInputs.Add(combo.inputList[i].action);
                }
            }
        }

        foreach (InputAction input in _allInputs)
        {
            input.performed += StartInput;
            input.canceled += CancelInput;
        }
        
        RestartCombo();
    }

    private void Update()
    {
        if (GameManager.Instance.currentGameState == GameState.Cutscene) return;
        //detect pause input
        if (elapsedTime > transitionDuration)
        {
            elapsedTime = 0f;
            if (currentCombo.Count > 0) CheckValidCombo(pauseInput);
        }
        else
        {
            elapsedTime += Time.deltaTime;
            canChainInput = true;
        }

        //detect hold input
         if (isHoldPossible)
         {
             holdTime += Time.deltaTime;
             if (holdTime >= holdMinDuration)
             {
                 isHoldPossible = false;
                 isHoldFinished = true;
                 elapsedTime = 0f;
                 if (HoldInputContainer == attackInput.action)
                 {
                     CheckValidCombo(holdInput);
                 }
             }
         }
    }

    private void StartInput(InputAction.CallbackContext callback)
    {
        isHoldPossible = true;
        HoldInputContainer = callback.action;
    }

    private void CancelInput(InputAction.CallbackContext callback)
    {
        isHoldPossible = false;
        holdTime = 0f;
        isHoldFinished = false;
        HoldInputContainer = NoneInputContainer;
        LookTarget?.Invoke();
        if (IsPlayingAttack())
        {
            if (NextInputContainer == NoneInputContainer)
            {
                NextInputContainer = callback.action;
            }
        }
        else
        {
            if (!isHoldFinished) CheckValidCombo(callback.action);
        }
    }

    public static event Action<ComboScriptableObject> LastInput;
    [HideInInspector] public UnityEvent LastInputEvent;
    private void CheckValidCombo(InputAction lastAction)
    {
        // Check if we are in play mode
        if (!GameManager.Instance.IsPlaying()) return;
        
        // Check if first attack
        if (!canChainInput) RestartCombo();
        
        if (lastAction == NoneInputContainer || lastAction == null) return;
        
        currentCombo.Add(lastAction);
        int currentComboLastIdx = currentCombo.Count - 1;
        bool inputEventSent = false;
        //check possible valid combos
        for (int i = validList.Count - 1; i >= 0; i--)
        {
            if (validList[i].inputList[currentComboLastIdx].action == lastAction)
            {
                if (!inputEventSent)
                {
                    //check if last input then Event to ANimation Behaviour
                    actionInput = validList[i].inputList[currentComboLastIdx];
                    
                    if (validList[i].inputList.Count == currentCombo.Count && !string.IsNullOrEmpty(validList[i].LastAnimation))
                    {
                        LastInput?.Invoke(validList[i]);
                        LastInputEvent?.Invoke();
                    }
                    else
                    {
                        //doAction, we keep the combo
                        InputEvent?.Invoke();
                    }
                    
                    inputEventSent = true;
                        
                }
            }
            else
            {
                //the combo isnt valid, it is thrown away
                validList.Remove(validList[i]);
            }
        }

        for (int i = validList.Count - 1; i >= 0; i--)
        {
            //remove finished combos
            IfFinishedCombos(validList[i]);
        }

        //else the list is kept
        elapsedTime = 0f;
        
        //check if there is any combo left
        if (validList.Count <= 0)
        {
            RestartCombo();
        }
    }

    private void IfFinishedCombos(ComboScriptableObject combo)
    {
        if (combo.inputList.Count <= currentCombo.Count)
        {
            finishedCombo = combo;
            FinishedComboEvent?.Invoke();
            validList.Remove(combo);
            NextInputContainer = null;
        }
    }

    public void FinishedAnimation()
    {
        if (NextInputContainer != NoneInputContainer)
        {
            CheckValidCombo(NextInputContainer);
            NextInputContainer = NoneInputContainer;
            elapsedTime = 0f;
        }
    }
    
    private bool IsPlayingAttack()
    {
        var animatorState = animator.GetCurrentAnimatorStateInfo(0);
        if (animatorState.normalizedTime < 1 && animatorState.IsTag("Attack"))
        {
            return true;
        }
        return false;
    }
    
    private void RestartCombo()
    {
        // Debug.Log("** restart **");
        currentCombo = new List<InputAction>();
        validList = new(allCombos);
        elapsedTime = transitionDuration + 1;
        canChainInput = false;
        currentAnimationName = null;
        CancelEvent?.Invoke();
    }
}