using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class BlancoCombatManager : MonoBehaviour
{
    public static BlancoCombatManager Instance
    {
        get
        {
            if (_instance == null) _instance = FindObjectOfType<BlancoCombatManager>();
            return _instance;
        }
    }

    private static BlancoCombatManager _instance;
    [SerializeField] private Animator animator;

    [HideInInspector] public UnityEvent InputEvent;
    [HideInInspector] public UnityEvent CancelEvent;
    [HideInInspector] public UnityEvent FinishedComboEvent;
    
    [Header("Inputs")]
    public InputActionReference pauseInput;
    public InputActionReference attackInput;
    public InputActionReference gunInput;
    public InputActionReference holdInput;
    private InputAction nextInput;
    private InputAction NoneInput;
    
    public InputAction actionInput { get; private set; }
    public ComboScriptableObject finishedCombo { get; private set; }

    [Header("Settings")]
    [SerializeField] private float transitionDuration;
    [SerializeField] private float holdMinDuration;
    [SerializeField] private List<ComboScriptableObject> allCombos = new List<ComboScriptableObject>();

    private List<ComboScriptableObject> validList = new List<ComboScriptableObject>();
    private List<InputAction> currentCombo = new List<InputAction>();
    private float elapsedTime;
    private float holdTime;
    private bool isHoldPossible;
    private bool isHoldFinished;
    private bool canChainInput;
    private const string INPUT_NONE = "None_Input";
    private void Start()
    {
        CharacterAnimatorEvents.OnEndAnimation += FinishedAnimation;
        NoneInput = new InputAction(INPUT_NONE);
        nextInput = NoneInput;
        
        elapsedTime = transitionDuration +1;
        //detect all inputs in scriptableObjects
        //may add a whitelist if needed
        List<InputAction> _allInputs = new List<InputAction>();
        foreach (ComboScriptableObject combo in allCombos)
        {
            for (int i = 0; i < combo.inputList.Count; i++)
            {
                //register to attacks
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

        if (isHoldPossible)
        {
            holdTime += Time.deltaTime;
            if (holdTime >= holdMinDuration)
            {
                isHoldPossible = false;
                isHoldFinished = true;
                CheckValidCombo(holdInput);
            }
        }
    }

    private void StartInput(InputAction.CallbackContext callback)
    {
        isHoldPossible = true;
    }

    private void CancelInput(InputAction.CallbackContext callback)
    {
        if (IsPlayingAttack())
        {
            if (nextInput == NoneInput)
            {
                Debug.Log(1);
                nextInput = callback.action;
            }
            else
            {
                Debug.Log(2);
                return;
            }
        }
        else
        {
            Debug.Log(3);
            if (!isHoldFinished) CheckValidCombo(callback.action);
        }

        isHoldFinished = false;
        isHoldPossible = false;
        holdTime = 0f;
    }

    private void CheckValidCombo(InputAction lastAction)
    {
        Debug.Log(1);
        if (lastAction == NoneInput || lastAction == null) return;
        
        //check if first attack
        if (!canChainInput) RestartCombo();
        
        Debug.Log(lastAction.name+" IS COMING");   
        
        Debug.Log(2);
        List<ComboScriptableObject> shitList = new List<ComboScriptableObject>();
        
        currentCombo.Add(lastAction);
        int currentComboLastIdx = currentCombo.Count - 1;
        bool inputEventSent = false;
        //check possible valid combos
        // for (int i = validList.Count - 1; i >= 0; i--)
        
        Debug.Log(3);
        for (int i = 0; i < validList.Count; i++)
        {
            Debug.Log($"Est ce que l'action {validList[i].inputList[currentComboLastIdx].action.name} est identique à l'action {lastAction.name}");
            if (validList[i].inputList[currentComboLastIdx].action == lastAction)
            {
                if (!inputEventSent)
                {
                    //doAction, we keep the combo
                    actionInput = validList[i].inputList[currentComboLastIdx];
                    InputEvent?.Invoke();
                    inputEventSent = true;
                }
            }
            else
            {
                //the combo isnt valid, it is thrown away
                shitList.Add(validList[i]);
                // validList.Remove(validList[i]);
            }
        }
        Debug.Log(4);

        for (int i = 0; i < validList.Count; i++)
        {
            //remove finished combos
            IfFinishedCombos(validList[i], shitList);
        }
        Debug.Log(5);
        
        //else the list is kept
        elapsedTime = 0f;
        
        for (int i = shitList.Count -1; i >= 0 ; i--)
        {
            validList.Remove(shitList[i]);
        }
        Debug.Log(6);

        //check if there is any combo left
        if (validList.Count <= 0)
        {
            RestartCombo();
        }
        Debug.Log(7);
    }

    private void IfFinishedCombos(ComboScriptableObject combo, List<ComboScriptableObject> shitList)
    {
        if (combo.inputList.Count <= currentCombo.Count)
        {
            finishedCombo = combo;
            FinishedComboEvent?.Invoke();
            // validList.Remove(combo);
            shitList.Add(combo);
            nextInput = null;
            // Debug.Log("BRAVO ! Vous avez terminé le combo "+combo.comboName);
        }
    }

    public void FinishedAnimation()
    {
        if (nextInput != NoneInput)
        {
            CheckValidCombo(nextInput);
            nextInput = NoneInput;
            elapsedTime = 0f;
        }
    }
    
    public bool IsPlayingAttack()
    {
        var animatorState = animator.GetCurrentAnimatorStateInfo(0);
        if (animatorState.normalizedTime < 1 && animatorState.IsTag("Attack"))
        {
            return true;
        }
        return false;
    }

    //delete current combo & possible combos
    private void RestartCombo()
    {
        // Debug.Log("** restart **");
        currentCombo = new List<InputAction>();
        validList = new(allCombos);
        elapsedTime = transitionDuration + 1;
        canChainInput = false;
        CancelEvent?.Invoke();
    }

    private void OnEnable()
    {
        CharacterAnimatorEvents.OnEndAnimation += FinishedAnimation;
    }

    private void OnDisable()
    {
        CharacterAnimatorEvents.OnEndAnimation -= FinishedAnimation;
    }
}