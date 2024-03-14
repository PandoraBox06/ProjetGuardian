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
    private int InputIndex = 0;
    private float elapsedTime;

    [SerializeField] private InputActionReference attackInput;

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
        InputIndex++;
        InputAction animInput = managerInstance.actionInput;

        string actionType = "";
        string inputType = "";

        switch (managerInstance.actionType)
        {
            case ActionType.Pause:
                return;
            case ActionType.Hold:
                inputType = "hold";
                if (animInput == attackInput.action) actionType = "Attack";
                else Debug.LogWarning("The attack "+animInput+" has no animation");
                return;//break; TO CHANGE <===============================================
            case ActionType.Simple:
                inputType = "simple";
                if (animInput == attackInput.action) actionType = "Attack";
                else Debug.LogWarning("The attack "+animInput+" has no animation");
                break;
        }
        
        TryToPlay($"{actionType}_{inputType}_{InputIndex}");
    }

    private void CancelAnimation()
    {
        InputIndex = 0;
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
        
        animator.Play(_animation);
    }

    private void OnDestroy()
    {
        if (this == Instance)
            Instance = null;
    }
}

public enum ActionType
{
    Simple,
    Hold,
    Pause
}