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

    [HideInInspector] public UnityEvent InputEvent;
    [HideInInspector] public UnityEvent CancelEvent;
    [HideInInspector] public UnityEvent FinishedComboEvent;
    public InputActionReference pauseInput;
    public InputActionReference attackInput;
    public InputActionReference holdInput;
    [SerializeField] private Animator animator;
    public InputAction actionInput { get; private set; }
    public ComboScriptableObject finishedCombo { get; private set; }
    public bool isHold { get; private set; }

    [Header("Settings")]
    [SerializeField] private float transitionDuration;
    [SerializeField] private float holdMinDuration;
    [SerializeField] private List<ComboScriptableObject> allCombos = new List<ComboScriptableObject>();

    private List<ComboScriptableObject> validList = new List<ComboScriptableObject>();
    private List<InputAction> currentCombo = new List<InputAction>();
    private float elapsedTime;
    private float holdTime;
    private bool isHoldPossible;
    private bool canChainInput;
    private InputAction nextInput;
    private const string INPUT_NONE = "None";
    private InputAction NoneInput;
    private void Start()
    {
        CharacterAnimatorEvents.OnEndAnimation += FinishedAnimation;
        NoneInput = new InputAction("INPUT_NONE");
        nextInput = NoneInput;
        
        elapsedTime = transitionDuration +1;
        //detect all inputs in scriptableObjects
        //may add a whitelist if needed
        List<InputAction> _simpleInputs = new List<InputAction>();
        List<InputAction> _holdInputs = new List<InputAction>();
        foreach (ComboScriptableObject combo in allCombos)
        {
            for (int i = 0; i < combo.inputList.Count; i++)
            {
                //register to simple attacks
                if (combo.inputList[i] == attackInput && !_simpleInputs.Contains(combo.inputList[i].action))
                {
                    _simpleInputs.Add(combo.inputList[i].action);
                }
                //register to hold attacks
                else if (combo.inputList[i] == holdInput &&
                         !_holdInputs.Contains(combo.inputList[i].action))
                {
                    _holdInputs.Add(combo.inputList[i].action);
                }
            }
        }

        foreach (InputAction input in _simpleInputs)
        {
            input.performed += StartInput;
            input.canceled += CancelInput;
        }

        foreach (InputAction input in _holdInputs)
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
            
            //I HATE YOU
            if (currentCombo.Count > 0) CheckValidCombo(pauseInput, false);
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
                isHold = true;
            }
        }
    }

    private void StartInput(InputAction.CallbackContext callback)
    {
        isHoldPossible = true;
    }

    private void CancelInput(InputAction.CallbackContext callback)
    {
        Debug.Log($"CancelInput>> Received input {callback.action.name}");
        if (IsPlayingAttack())
        {
            Debug.Log($"\tCancelInput>> an animation is already playing : {animator.GetCurrentAnimatorStateInfo(0).ToString()}");
            if (nextInput == NoneInput)
            {
                Debug.Log($"\tCancelInput>> Regisering as next input");
                Debug.LogWarning($"nextInput is {callback.action.name}");
                nextInput = callback.action;
            }
            else
            {
                Debug.Log("Cancelling input because a buffered input already exists : " + nextInput.name);
                return;
            }
        }
        else
        {
            Debug.Log($"No attack playing, checking input {callback.action.name} for combos");
            CheckValidCombo(callback.action, false);//isHold);
        }

        isHoldPossible = false;
        isHold = false;
        holdTime = 0f;
    }

    private void CheckValidCombo(InputAction lastAction, bool isHold)
    {
        Debug.Log("========>> On joue "+lastAction.name);
        List<ComboScriptableObject> shitList = new List<ComboScriptableObject>();
        
        //check if first attack
        if (!canChainInput) RestartCombo();
        
        currentCombo.Add(lastAction);
        int currentComboLastIdx = currentCombo.Count - 1;
        bool inputEventSent = false;
        //check possible valid combos
        // for (int i = validList.Count - 1; i >= 0; i--)
        
        for (int i = 0; i < validList.Count; i++)
        {
            // Debug.Log($"if {validList[i].inputList[currentCombo.Count].action.name} == {currentCombo[i].name}");
            // Debug.Log($"{i} == {currentCombo.Count} == {validList[i].inputList.Count}");
            // Debug.Log(validList[i].inputList[currentCombo.Count]);
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

        for (int i = 0; i < validList.Count; i++)
        {
            //remove finished combos
            IfFinishedCombos(validList[i], shitList);
        }
        
        //else the list is kept
        elapsedTime = 0f;
        
        //ICIIIIIIIIII LAAAAA
        for (int i = shitList.Count -1; i >= 0 ; i--)
        {
            validList.Remove(shitList[i]);
        }

        //check if there is any combo left
        if (validList.Count <= 0)
        {
            RestartCombo();
        }
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
            Debug.Log("BRAVO ! Vous avez terminé le combo "+combo.comboName);
        }
    }

    public void FinishedAnimation()
    {
        if (nextInput != NoneInput)
        {
            Debug.Log("on joue l'anim suivante : "+nextInput.name);
            CheckValidCombo(nextInput, false);
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
        Debug.Log("** restart **");
        currentCombo = new List<InputAction>();
        validList = new(allCombos);
        elapsedTime = transitionDuration + 1;
        canChainInput = false;
        CancelEvent?.Invoke();
    }
}