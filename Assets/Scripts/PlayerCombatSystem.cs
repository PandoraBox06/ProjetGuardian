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
    public Weapon weapon;
    public InputActionReference attackAction;
    public Transform orientation;

    [Header("AutoLock Target")]
    Transform closestTarget;
    public float enemyClosestRadius = 1f;
    public float minimumRadius = .5f;
    public LayerMask enemyMask;

    private void OnEnable()
    {
        attackAction.action.performed += NormalAttack;
    }

    private void OnDisable()
    {
        attackAction.action.performed -= NormalAttack;
    }

    void Update()
    {
        ExitAttack();
        ClosestEnemy();
        LookAtEnemy();
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
                DealDamage();
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

    public void DealDamage()
    {
        Collider[] colliders = new Collider[99];
        int numColliders = Physics.OverlapSphereNonAlloc(weapon.hitbox.position, weapon.radius, colliders, enemyMask);
        for (int i = 0; i < numColliders; i++)
        {
            IDamageable damagable = colliders[i].GetComponent<IDamageable>();
            damagable?.TakeDamage(weapon.damage);
        }
    }

    void EndCombo()
    {
        comboCounter = 0;
        lastComboEnd = Time.time;
    }

    void ClosestEnemy()
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position + Vector3.up, enemyClosestRadius, enemyMask);

        for (int i = 0; i < enemies.Length; i++)
        {
            if (Vector3.Distance(transform.position, enemies[i].transform.position) <= minimumRadius)
            {
                if (closestTarget == null)
                {
                    closestTarget = enemies[i].transform;
                    break;
                }
                else
                {
                    return;
                }
            }
        }
    }

    void LookAtEnemy()
    {
        if (closestTarget != null)
        {
            transform.LookAt(closestTarget);
            orientation.LookAt(closestTarget);
        }

        if (closestTarget != null && Vector3.Distance(transform.position + Vector3.up, closestTarget.position) > enemyClosestRadius)
            closestTarget = null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + Vector3.up, enemyClosestRadius);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position + Vector3.up, minimumRadius);
    }
}
