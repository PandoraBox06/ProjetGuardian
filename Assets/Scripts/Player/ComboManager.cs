using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
public class Combo
{
    public InputActionReference[] comboList;
}

public class ComboManager : MonoBehaviour
{
    [SerializeField] private InputActionReference meleeAttack;
    [SerializeField] private InputActionReference rangeAttack;
    [SerializeField] private InputActionReference pauseAttack;
    
    public List<Combo> comboNormalAttack;
    float lastClickedTime;
    public float clickTimeCooldown;
    float lastComboEnd;
    public float comboCooldown;
    int comboCounter;

   /*[HideInInspector]*/ public Animator animator;

    Transform closestTarget;
    public float enemyClosestRadius = 1f;
    public float minimumRadius;
    public LayerMask enemyMask;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }
    private void OnEnable()
    {
        meleeAttack.action.performed += NormalAttack;
        rangeAttack.action.performed += RangeAttack;
    }

    private void OnDisable()
    {
        meleeAttack.action.performed -= NormalAttack;
        rangeAttack.action.performed -= RangeAttack;
    }

    void Update()
    {
        ExitAttack();
        // ClosestEnemy();
        // LookAtEnemy();
    }

    void NormalAttack(InputAction.CallbackContext context)
    {
        if (Time.time - lastComboEnd > comboCooldown)
        {
            CancelInvoke("EndCombo");

            if (Time.time - lastClickedTime >= clickTimeCooldown)
            {
                comboCounter++;
                animator.SetTrigger("Attack" + comboCounter);
                lastClickedTime = Time.time;

                if (comboCounter >= comboNormalAttack[0].comboList.Length)
                {
                    EndCombo();
                }
            }
        }
    }

    void RangeAttack(InputAction.CallbackContext context)
    {
        if (Time.time - lastComboEnd > comboCooldown)
        {
            CancelInvoke("EndCombo");

            if (Time.time - lastClickedTime >= clickTimeCooldown)
            {
                animator.SetTrigger("Range");
                lastClickedTime = Time.time;

                EndCombo();
            }
        }
    }

    void ExitAttack()
    {
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.9f && animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
        {
            Invoke("EndCombo", 1);
        }
    }

    void EndCombo()
    {
        comboCounter = 0;
        lastComboEnd = Time.time;
    }

    // void ClosestEnemy()
    // {
    //     Collider[] enemies = Physics.OverlapSphere(transform.position, enemyClosestRadius, enemyMask);
    //
    //     for (int i = 0; i < enemies.Length; i++)
    //     {
    //         if (Vector3.Distance(transform.position, enemies[i].transform.position) <= minimumRadius)
    //         {
    //             if(closestTarget == null)
    //             {
    //                 closestTarget = enemies[i].transform;
    //                 break;
    //             }
    //             else
    //             {
    //                 return;
    //             }
    //         }
    //     }
    // }
    //
    // void LookAtEnemy()
    // {
    //     if (closestTarget != null)
    //         transform.LookAt(closestTarget);
    //
    //     if (closestTarget != null && Vector3.Distance(transform.position, closestTarget.position) > enemyClosestRadius)
    //         closestTarget = null;
    // }

    private void OnDrawGizmos()
    {
        //Gizmos.color = Color.red;
        //Gizmos.DrawWireSphere(transform.position, enemyClosestRadius);
        //Gizmos.color = Color.cyan;
        //Gizmos.DrawWireSphere(transform.position, minimumRadius);
    }
}
