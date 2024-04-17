using UnityEngine;
using UnityEngine.InputSystem;

public class BlancoAnimationBehaviour : MonoBehaviour
{
    public static BlancoAnimationBehaviour Instance { get; private set; }
    [SerializeField] private Animator animator; //to pass as parameter

    private Fuckall managerInstance;
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

        managerInstance = Fuckall.Instance;
        // managerInstance.CancelEvent.AddListener(CancelAnimation);
    }

    // private void CancelAnimation()
    // {
    //     inputIndex = 0;
    //     animator.SetTrigger("CancelAnimation");
    //     
    // }

    public void TryToPlay(string _animation)
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

    public void FollowWithInput(ActionType input)
    {
        int index = 0;
        if (input == ActionType.Attack) index = 1;
        else if (input == ActionType.Range) index = 2;
        
        animator.SetInteger("InputType", index);
        animator.SetTrigger("NewInput");
    }

    private void OnDestroy()
    {
        if (this == Instance)
            Instance = null;
    }
}

public enum ActionType
{
    Attack,
    Hold,
    Range,
    Pause
}