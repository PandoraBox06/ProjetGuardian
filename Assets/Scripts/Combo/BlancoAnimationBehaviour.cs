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

    private void OnEnable()
    {
        BlancoCombatManager.LastInput += PlayLastComboAnim;
    }

    private void OnDisable()
    {
        BlancoCombatManager.LastInput -= PlayLastComboAnim;
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
            // inputType = "pause";
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
        inputIndex = 0;
        animator.SetTrigger("CancelAnimation");
    }

    private void TryToPlay(string _animation)
    {
        //chck si string pour last sinon go
        var stateID = Animator.StringToHash(_animation);
        var hasState = animator.HasState(0, stateID);
        if (!hasState)
        {
            Debug.LogWarning($"The animation {_animation} doesn't exist, you made a mistake");
            return;
        }
        
        // Debug.Log(_animation + " has been triggered");
        animator.Play(_animation);
    }

    private void PlayLastComboAnim(ComboScriptableObject combo)
    {
        if (string.IsNullOrEmpty(combo.LastAnimation))
        {
            return;
        }

        animator.Play(combo.LastAnimation);
    }
    
    private void OnDestroy()
    {
        if (this == Instance)
            Instance = null;
    }
}