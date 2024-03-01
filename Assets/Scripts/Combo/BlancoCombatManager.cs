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
    [HideInInspector] public UnityEvent NextAnimEvent;
    [SerializeField] private InputActionReference pauseInput;
    public ActionType actionType { get; private set; }
    public InputAction actionInput { get; private set; }
    public string actionName { get; private set; }
    public bool canChainInput { get; private set; }
    public bool isHold { get; private set; }

    [Header("Settings")] [SerializeField] private float transitionDuration;
    [SerializeField] private float holdMinDuration;
    [SerializeField] private List<ComboScriptableObject> allCombos = new List<ComboScriptableObject>();

    public List<ComboScriptableObject> validList = new List<ComboScriptableObject>();
    private List<InputAction> currentCombo = new List<InputAction>();
    public float elapsedTime;
    private float holdTime;
    private bool isHoldPossible;

    private void Start()
    {
        elapsedTime = transitionDuration;
        //detect all inputs in scriptableObjects
        //may add a whitelist if needed
        List<InputAction> _simpleInputs = new List<InputAction>();
        List<InputAction> _holdInputs = new List<InputAction>();
        foreach (ComboScriptableObject combo in allCombos)
        {
            for (int i = 0; i < combo.inputList.Count; i++)
            {
                //register to simple attacks
                if (combo.actionTypesList[i] == ActionType.Simple && !_simpleInputs.Contains(combo.inputList[i].action))
                {
                    _simpleInputs.Add(combo.inputList[i].action);
                }
                //register to hold attacks
                else if (combo.actionTypesList[i] == ActionType.Hold &&
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
            Debug.Log("should happen once");
            elapsedTime = 0f;
            
            Debug.Log(currentCombo);
            Debug.Log(pauseInput);
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
        Debug.Log($"New input : {callback.action.name}");
        CheckValidCombo(callback.action, false);//isHold);

        isHoldPossible = false;
        isHold = false;
        holdTime = 0f;
    }

    private void CheckValidCombo(InputAction lastAction, bool isHold)
    {
        Debug.Log(1);
        bool isPauseAvailable = false;
        //check if first attack
        if (!canChainInput) RestartCombo();
        Debug.Log(2);
        
        currentCombo.Add(lastAction);
        Debug.Log(3);

        int comboCount = currentCombo.Count;
        Debug.Log(4);

        //check possible valid combos
        for (int i = validList.Count - 1; i >= 0; i--)
        {
            //remove finished combos
            if (IfFinishedCombos(validList[i]))
            {
                continue;
            }

            Debug.Log($"loop n°{i} current combo count =>> {currentCombo.Count}");
            for (int j = 0; j < currentCombo.Count; j++)
            {
                Debug.Log(j + " : " + currentCombo[j].name);
            }

            // Debug.Log($"if {validList[comboCount].inputList[currentCombo.Count].action.name} == {currentCombo.Last().name}");
            if (validList[i].inputList[currentCombo.Count].action == currentCombo.Last())
            {
                //doAction, we keep the combo
                actionName = validList[i].comboName;
                actionInput = validList[i].inputList[currentCombo.Count];
                actionType = validList[i].actionTypesList[currentCombo.Count];
                InputEvent?.Invoke();
                if (actionType == ActionType.Pause) isPauseAvailable = true;
                break;
            }
            else
            {
                //the combo isnt valid, it is thrown away
                validList.Remove(validList[i]);
            }
        }

        if (actionType == ActionType.Pause && !isPauseAvailable)
        {
            canChainInput = false;
            RestartCombo();
        }

        //check if there is any combo left
        if (validList.Count <= 0)
        {
            RestartCombo();
        }

        //else the list is kept
        elapsedTime = 0f;
    }

    private bool IfFinishedCombos(ComboScriptableObject combo)
    {
        if (combo.inputList.Count <= currentCombo.Count)
        {
            validList.Remove(combo);
            Debug.Log("BRAVO ! Vous avez terminé le combo "+combo.comboName);
            return true;
        }

        return false;
    }

    public void FinishedAnimation()
    {
        NextAnimEvent?.Invoke();
    }

    //delete current combo & possible combos
    private void RestartCombo()
    {
        Debug.Log("** restart **");
        currentCombo = new List<InputAction>();
        validList = new(allCombos);
        elapsedTime = transitionDuration;
        CancelEvent?.Invoke();
    }
}