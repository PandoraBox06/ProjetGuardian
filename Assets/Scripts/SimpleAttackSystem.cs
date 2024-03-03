using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SimpleAttackSystem : MonoBehaviour
{
    public static SimpleAttackSystem Instance { get; private set;}
    [SerializeField] private InputActionReference attackAction;
    [SerializeField] private InputActionReference rangeAction;
    [SerializeField] private InputActionReference dashAction;
    [SerializeField] private Animator _animator;
    public string comboName = "Atk";
    [SerializeField] private string shootingName = "Shooting";
    public int comboCount = 0;
    [SerializeField] private int maxComboCount = 0;

    [SerializeField] private float resetComboTimer = 1f;
    [SerializeField] private float timeBetweenAttack = .5f;
    private float timer;

    public bool isAttaking;

    [Header("Range")]
    public GameObject projectilePrefab;
    public float projectileSpeed;
    public Transform shootingOutput;
    public Transform projectileDump;
    public CombatCamBehavior combatCamBehavior;

    private void Awake()
    {
        if (Instance)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    private void OnDestroy()
    {
        if (this == Instance)
            Instance = null;
    }
    
    private void OnEnable()
    {
        attackAction.action.performed += Attack;
        rangeAction.action.performed += Range;
        dashAction.action.performed += Dash;
        CharacterAnimatorEvents.OnFireProjectile += FireProjectile;
    }

    private void OnDisable()
    {
        attackAction.action.performed -= Attack;
        rangeAction.action.performed -= Range;
        dashAction.action.performed -= Dash;
        CharacterAnimatorEvents.OnFireProjectile -= FireProjectile;
    }

    void Attack(InputAction.CallbackContext context)
    {
        if (Time.time > timer)
        {
            CancelInvoke(nameof(ResetCombo));
            
            comboCount++;
            // _animator.Play(comboName + comboCount);
            isAttaking = true;
            timer = Time.time + timeBetweenAttack;
            Invoke(nameof(ResetCombo), resetComboTimer);
            
            if (comboCount >= maxComboCount + 1)
                ResetCombo();
                
        }
    }
    
    void Range(InputAction.CallbackContext context)
    {
        if (Time.time > timer)
        {
            _animator.Play(shootingName);
            timer = Time.time + timeBetweenAttack;
        }
    }

    void FireProjectile()
    {
        var thisProjectile = Instantiate(projectilePrefab, shootingOutput.position, Quaternion.identity, projectileDump);
        Vector3 projectileDir = new();
        if(combatCamBehavior.closestTarget != null)
            projectileDir = combatCamBehavior.closestTarget.position - transform.position;
        else
            projectileDir = transform.forward;
        thisProjectile.GetComponent<Rigidbody>().AddForce(projectileDir * projectileSpeed, ForceMode.Impulse);
    }

    void ResetCombo()
    {
        comboCount = 0;
    }

    void Dash(InputAction.CallbackContext context)
    {
        _animator.CrossFade("Exit", 0.2f);
    }
}
