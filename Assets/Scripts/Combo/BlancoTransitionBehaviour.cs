using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class BlancoTransitionBehaviour : StateMachineBehaviour
{
    [SerializeField] private bool isFinal;
    [SerializeField] private string comboName;
    private float clipLength;
    private float elapsedTime;

    private Slider inputSlider;
    private bool hasInput;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Fuckall.Instance.InputEvent.AddListener(InputHasBeenDone);
        Fuckall.Instance.CanInput = true;
        inputSlider = Fuckall.Instance.inputTimingSlider;
        
        if (!inputSlider.IsActive())
        {
            inputSlider.gameObject.SetActive(true);
            elapsedTime = 0f;
        }
        
        clipLength = animator.GetCurrentAnimatorClipInfo(0)[0].clip.length;
        inputSlider.maxValue = clipLength;
    }

    private void InputHasBeenDone()
    {
        hasInput = true;
    }
    
    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        elapsedTime += Time.deltaTime;
        inputSlider.value = elapsedTime;
    }
    
    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Fuckall.Instance.CanInput = false;
        
        if (inputSlider.IsActive()) inputSlider.gameObject.SetActive(false);

        if (isFinal)
        {
            if (comboName != null) Fuckall.Instance.FinishCombo(comboName);
            else Debug.LogWarning("There is no name for this combo");
        }
    }
}