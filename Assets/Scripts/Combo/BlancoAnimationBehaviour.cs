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
        managerInstance.LastInputEvent.AddListener(StartLastAnimation);
        managerInstance.CancelEvent.AddListener(CancelAnimation);
    }

    private void StartAnimation()
    {
        inputIndex++;
        InputAction animInput = managerInstance.actionInput;

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

    private void StartLastAnimation()
    {
        ComboScriptableObject comboSO = managerInstance.LastAnimSO;

        string lastAnimName = comboSO.nameOfLastAnim;
        if (string.IsNullOrEmpty(lastAnimName))
        {
            //if there is no lastAnim custom we play the normal final anim
            StartAnimation();
        }
        
        inputIndex = 0;
        TryToPlay(lastAnimName);
    }

    private void CancelAnimation()
    {
        inputIndex = 0;
        animator.SetTrigger("CancelAnimation");
    }

    private bool IsPlaying()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1) return true;
        return false;
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

        animator.SetTrigger(_animation);
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