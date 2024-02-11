using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCombatSystem : MonoBehaviour
{
    [Header("MeleeCombo")]
    public List<SO_Attack> comboNormalAttack;
    float lastClickedTime;
    public float clickTimeCooldown;
    float lastComboEnd;
    public float comboCooldown;
    int comboCounter;

    [Header("Range")]
    public float shootingCooldown;
    public GameObject projectilePrefab;
    public float projectileSpeed;
    public Transform shootingOutput;
    public Transform projectileDump;
    float shootingTimer;

    [Header("References")]
    public Animator animator;
    public InputActionReference meleeAction;
    public InputActionReference rangeAction;
    public CombatCamBehavior combatCamBehavior;
    public Transform orientation;


    private void OnEnable()
    {
        CharacterAnimatorEvents.OnFireProjectile += FireProjectile;
        meleeAction.action.performed += NormalAttack;
        rangeAction.action.performed += RangeAttack;
    }

    private void OnDisable()
    {
        CharacterAnimatorEvents.OnFireProjectile -= FireProjectile;
        meleeAction.action.performed -= NormalAttack;
        rangeAction.action.performed -= RangeAttack;
    }

    void Update()
    {
        ExitAttack();
    }

    #region Melee
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
    #endregion

    #region Range
    void RangeAttack(InputAction.CallbackContext callbackContext)
    {
        if (Time.time > shootingTimer)
        {
            animator.Play("Shooting", 0, 0);
            shootingTimer = Time.time + shootingCooldown;
        }
    }

    void FireProjectile()
    {
        var thisProjectile = Instantiate(projectilePrefab, shootingOutput.position, Quaternion.identity, projectileDump);
        Vector3 projectileDir = new();
        if (combatCamBehavior.closestTarget != null)
            projectileDir = combatCamBehavior.closestTarget.position - transform.position;
        else projectileDir = transform.forward;
        thisProjectile.GetComponent<Rigidbody>().AddForce(projectileDir * projectileSpeed, ForceMode.Impulse);
    } 
    #endregion
}
