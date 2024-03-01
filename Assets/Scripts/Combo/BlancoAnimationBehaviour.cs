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
    private int InputIndex = 1;
    private float elapsedTime;

    private string nextAnimation;
    
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
        managerInstance.NextAnimEvent.AddListener(PlayNextAnimation);
    }

    private void Update()
    {
        // animationProgress = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        
        // Debug.Log(IsPlaying("Attack_sword_1"));
    }

    private void StartAnimation()
    {
        string _fullName = managerInstance.actionName;
        _fullName = _fullName.Substring(0, _fullName.Length-2);
        string _animationName = $"{_fullName}_{InputIndex}";
        // Debug.Log(_animationName);
        
        //if animation en cours
        if (IsPlaying() && nextAnimation == "")
        {
            nextAnimation = _animationName;
            return;
        }
        
        PlayAnimation(_animationName);
    }
    
    private void PlayNextAnimation()
    {
        if (nextAnimation == null)
        {
            animator.Play("Walking");
            return;
        }
        
        PlayAnimation(nextAnimation);

        nextAnimation = null;
    }

    private void PlayAnimation(string _animationName)
    {
        InputIndex++;

        switch (managerInstance.actionType)
        {
            case ActionType.Simple:
                TryToPlay(_animationName);
                break;
            case ActionType.Hold:
                TryToPlay(_animationName);
                break;
            case ActionType.Pause:
                break;
        }
    }

    private void CancelAnimation()
    {
        InputIndex = 1;
        animator.SetTrigger("CancelAnimation");
        
    }

    private bool IsPlaying(string stateName)
    {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName(stateName) &&
            animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f) return true;
        else return false;
    }

    private bool IsPlaying()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >
            animator.GetCurrentAnimatorStateInfo(0).length) return true;
        else return false;
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