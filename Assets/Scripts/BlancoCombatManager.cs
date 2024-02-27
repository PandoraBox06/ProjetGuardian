using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class BlancoCombatManager : MonoBehaviour
{
    
    public UnityEvent InputEvent;
    public static BlancoCombatManager Instance { get; private set; }
    [Header("Settings")]
    [SerializeField] private float transitionDuration;
    [SerializeField] private List<ComboScriptableObject> allCombos = new List<ComboScriptableObject>();
    [SerializeField] private List<ComboScriptableObject> validList = new List<ComboScriptableObject>();
    
    private List<InputAction> currentCombo = new List<InputAction>();
    public float elapsedTime;
    private bool canChainInput;
    
    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        elapsedTime = transitionDuration;
    }

    private void Update()
    {
        if (elapsedTime >= transitionDuration)
        {
            canChainInput = false;
        }
        else
        {
            elapsedTime += Time.deltaTime;
            canChainInput = true;
        }
    }

    private void Start()
    {
        //detect all inputs in scriptableObjects
        //may add a whitelist if needed
        List<InputAction> _allInputs = new List<InputAction>();
        foreach (ComboScriptableObject combo in allCombos)
        {
            foreach (InputActionReference input in combo.inputList)
            {
                if (!_allInputs.Contains(input.action)) _allInputs.Add(input.action);
            }
        }
        
        foreach (InputAction input in _allInputs)
        {
            input.performed += StartInput;
        }
    }

    private void StartInput(InputAction.CallbackContext callback)
    {
        Debug.Log($"New input : {callback.action.name}");
        if (IsItValidCombo(callback.action)) InputEvent?.Invoke();;
    }

    //true if combo exists with the given input
    private bool IsItValidCombo(InputAction lastAction)
    {
        //check if first attack
        Debug.Log($"can the combo chain? {canChainInput} ({elapsedTime}s)");
        if(!canChainInput) RestartCombo();
        currentCombo.Add(lastAction);

        Debug.Log("---");
        //check possible valid combos
        Debug.Log($"validList has a count of {validList.Count}");
        for (int i = validList.Count-1 ; i > 0; i--)
        {
            //remove finished combos
            if (IfFinishedCombos(validList[i])) { continue; }
            Debug.Log("done");
            
            // Debug.Log($"current inputList has a count of {validList[i].inputList.Count}");
            if (validList[i].inputList[currentCombo.Count].action == currentCombo.Last())
            {
                //doAction, we keep the combo
                // Debug.Log($"Yay ! the combo is kept [i = {i}]");
            }
            else
            {
                //the combo isnt valid, it is thrown away
                validList.Remove(validList[i]);
            }
        }
        Debug.Log("---");

        //check if there is any combo left
        if (validList.Count <= 0)
        {
            Debug.Log("This combo doesn't exist.");
            RestartCombo();
            return false;
        }

        //else the list is kept
        // Debug.Log($"There is {validList.Count} combos left");
        elapsedTime = 0f;
        return true;
    }

    private bool IfFinishedCombos(ComboScriptableObject combo)
    {
        Debug.Log($"ActionList count {combo.inputList.Count} == current combo count {currentCombo.Count}");
        if (combo.inputList.Count <= currentCombo.Count)
        {
            validList.Remove(combo);
            return true;
        }

        return false;
    }

    //delete current combo & possible combos
    private void RestartCombo()
    {
        Debug.Log("restart combo");
        currentCombo = new List<InputAction>();
        validList = new(allCombos);
        elapsedTime = transitionDuration;
    }
    
    private void OnDestroy()
    {
        if (this == Instance)
            Instance = null;
    }
}