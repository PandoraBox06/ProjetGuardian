using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombatSystem : MonoBehaviour
{
    [Header("Combo")]
    public List<SO_Attack> comboNormalAttack;
    float lastClickedTime;
    public float clickTimeCooldown;
    float lastComboEnd;
    public float comboCooldown;
    int comboCounter;

    [Header("References")]
    public Animator animator;
    public InputActionReference meleeAction;
    public InputActionReference rangeAction;
    public CombatCamBehavior combbatCamBehavior;


    private void OnEnable()
    {
        meleeAction.action.performed += NormalAttack;
    }

    private void OnDisable()
    {
        meleeAction.action.performed -= NormalAttack;
    }

    void Update()
    {
        ExitAttack();
    }

    void NormalAttack(InputAction.CallbackContext callbackContext)
    {
        if (Time.time - lastComboEnd > comboCooldown && comboCounter <= comboNormalAttack.Count)
        {
            CancelInvoke(nameof(EndCombo));

            if (Time.time - lastClickedTime >= clickTimeCooldown)
            {
                animator.runtimeAnimatorController = comboNormalAttack[comboCounter].animatorOV; ;
                animator.Play("Attack", 0, 0);
                comboCounter++;
                lastClickedTime = Time.time;

                if (comboCounter >= comboNormalAttack.Count)
                {
                    EndCombo();
                }
            }
        }
    }

    void ExitAttack()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9f && animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
        {
            Invoke(nameof(EndCombo), 1);
        }
    }

    void EndCombo()
    {
        comboCounter = 0;
        lastComboEnd = Time.time;
    }
}
