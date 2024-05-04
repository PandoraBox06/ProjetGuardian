using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class BlancoAnimationBehaviour : MonoBehaviour
{
    public static BlancoAnimationBehaviour Instance { get; private set; }
    [SerializeField] private Animator animator; //to pass as parameter

    private BlancoCombatManager managerInstance;
    private float animationProgress;
    private int inputIndex;
    private float elapsedTime;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        managerInstance = BlancoCombatManager.Instance;
        managerInstance.InputEvent.AddListener(StartAnimation);
        managerInstance.CancelEvent.AddListener(CancelAnimation);
    }
    
    private void StartAnimation()
    {
        inputIndex++;
        
        string actionType = "Attack";
        string inputType = "";
        
        if (managerInstance.actionInput == managerInstance.attackInput.action)
        {
            inputType = "simple";
        }
        else if (managerInstance.actionInput == managerInstance.pauseInput.action)
        {
            return;
        }
        else if (managerInstance.actionInput == managerInstance.holdInput.action)
        {
            inputType = "hold";
        }
        else if (managerInstance.actionInput == managerInstance.gunInput.action)
        {
            inputType = "gun";
        }
        
        TryToPlay($"{actionType}_{inputType}_{inputIndex}");
    }

    private void CancelAnimation()
    {
        // foreach (var trigger in animator.parameters)
        // {
        //     if (trigger.type == AnimatorControllerParameterType.Trigger)
        //     {
        //         animator.ResetTrigger(trigger.name);
        //     }
        // }
        
        inputIndex = 0;
        animator.SetTrigger("CancelAnimation");
    }

    private void TryToPlay(string _animation)
    {
        var stateID = Animator.StringToHash(_animation);
        var hasState = animator.HasState(0, stateID);
        if (!hasState)
        {
            Debug.LogWarning($"The animation {_animation} doesn't exist, you made a mistake");
            return;
        }
        
        // foreach (var trigger in animator.parameters)
        // {
        //     if (trigger.type == AnimatorControllerParameterType.Trigger)
        //     {
        //         animator.ResetTrigger(trigger.name);
        //     }
        // }
        
        // Debug.Log(_animation + " has been triggered");
        animator.SetTrigger(_animation);
    }

    private void OnDestroy()
    {
        if (this == Instance)
            Instance = null;
    }
}