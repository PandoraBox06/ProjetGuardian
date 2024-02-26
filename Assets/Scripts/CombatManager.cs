using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CombatManager : MonoBehaviour
{
    public static CombatManager instance;
    [HideInInspector] public bool isAttacking = false;
    [HideInInspector] public bool isShooting = false;
    [SerializeField] InputActionReference attackAction;
    [SerializeField] InputActionReference rangeAction;
    [SerializeField] InputActionReference dashAction;
    [SerializeField] InputActionReference pauseAction;
    [SerializeField] InputActionReference[] combo1;
    [SerializeField] InputActionReference[] combo2;
    [SerializeField] InputActionReference[] combo3;
    [SerializeField] InputActionReference[] combo4;
    [HideInInspector] public List<InputAction> parsing;
    [HideInInspector] public int comboCount1;
    [HideInInspector] public int comboCount2;
    [HideInInspector] public int comboCount3;
    [HideInInspector] public int comboCount4;
    [HideInInspector] public bool firstAttack;
    [HideInInspector] public bool canParse;
    [HideInInspector] public bool isDashing;
    [HideInInspector] public bool isHoldingAttack;

    [Header("Range")]
    [SerializeField] float shootingCooldown;
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] float projectileSpeed;
    [SerializeField] Transform shootingOutput;
    [SerializeField] Transform projectileDump;
    float shootingTimer;
    [SerializeField] CombatCamBehavior combatCamBehavior;
    [SerializeField] Transform orientation;

    [Header("VFX")]
    [SerializeField] GameObject VFX_Slash;
    [SerializeField] GameObject VFX_Tir;

    private void Awake()
    {
        instance = this;
        parsing = new List<InputAction>();
    }

    private void OnEnable()
    {
        rangeAction.action.started += Range;
        dashAction.action.started += Dash;
        CharacterAnimatorEvents.OnFireProjectile += FireProjectile;
    }
    private void OnDisable()
    {
        rangeAction.action.started -= Range;
        dashAction.action.started -= Dash;
        CharacterAnimatorEvents.OnFireProjectile -= FireProjectile;
    }

    private void Start()
    {
        if (VFX_Slash != null)
        {
            VFX_Slash.SetActive(false);
        }

        if (VFX_Tir != null)
        {
            VFX_Tir.SetActive(false);

        }
    }

    void VFXSlash()
    {
        VFX_Slash.SetActive(true);
        //  Debug.Log("Oui");

    }
    void VFXTir()
    {
        VFX_Tir.SetActive(true);
        //  Debug.Log("Oui");

    }

    IEnumerator DeactivateVFXSlash()
    {

        yield return new WaitForSeconds(1);

        VFX_Slash.SetActive(false);
    }

    IEnumerator DeactivateVFXTir()
    {

        yield return new WaitForSeconds(0.5f);

        VFX_Tir.SetActive(false);
    }


    private void Update()
    {
        if(!isAttacking || !isShooting)
        {
            if(attackAction.action.IsPressed())
                timer += Time.deltaTime;
            if (attackAction.action.WasReleasedThisFrame())
            {
                Attack();
            }
        }
    }

    float timer;
    public void Attack()
    {   
        if(timer >= 0.6f)
        {
            isHoldingAttack = true;
            timer = 0;
        }
        else
        {
            if (canParse)
                parsing.Add(attackAction.action);
            else if (!firstAttack)
            {
                comboCount1++;
                comboCount2++;
                comboCount3++;
                comboCount4++;
                isAttacking = true;
                firstAttack = true;
                Invoke(nameof(VFXSlash), 0.6f);

                StartCoroutine(DeactivateVFXSlash());
                timer = 0;
            }
        }        


        if (isHoldingAttack && !canParse)
        {
            isAttacking = true;
            firstAttack = true;
            isHoldingAttack = false;
            timer = 0;
            Debug.Log("Heavy Attack Playin'");
            //Play Heavy Attack
        }
        
    }

    #region Range
    public void Range(InputAction.CallbackContext context)
    {
        if (!isAttacking || !isShooting)
        {
            if (Time.time > shootingTimer)
            {
                if (canParse)
                    parsing.Add(rangeAction.action);
                else if (!firstAttack)
                {
                    comboCount1++;
                    comboCount2++;
                    comboCount3++;
                    comboCount4++;
                    isShooting = true;
                    firstAttack = true;
                }
            }
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

        Invoke("VFXTir", 0);
        StartCoroutine(DeactivateVFXTir());
    } 
    #endregion

    public void Dash(InputAction.CallbackContext context)
    {
        if (canParse)
            parsing.Add(dashAction.action);
        else if (!firstAttack)
        {
            isDashing = true;
            firstAttack = true;
            comboCount1++;
            comboCount2++;
            comboCount3++;
            comboCount4++;
        }
    }

    public void PauseAction()
    {
        parsing.Add(pauseAction.action);
    }

    public void CheckParse()
    {
        for (int i = 0; i < parsing.Count; i++)
        {
            if(comboCount1 < combo1.Length && parsing[i] == combo1[comboCount1].action)
            {
                comboCount1++;
                isAttacking = true;
                parsing.Clear();
            }
            else if (comboCount2 < combo2.Length && parsing[i] == combo2[comboCount2].action)
            {
                comboCount2++;
                isAttacking = true;
                parsing.Clear();
            }
            else if (comboCount3 < combo3.Length && parsing[i] == combo3[comboCount3].action)
            {
                comboCount3++;
                isAttacking = true;
                parsing.Clear();
            }
            else if (comboCount4 < combo4.Length && parsing[i] == combo4[comboCount4].action)
            {
                comboCount4++;
                isAttacking = true;
                parsing.Clear();
            }
            else if (parsing.Count <= 0)
            {
                ClearParse();
            }
            else
                ClearParse();

        }
    }

    public void ClearParse()
    {
        comboCount1 = 0;
        comboCount2 = 0;
        comboCount3 = 0;
        comboCount4 = 0;
        isAttacking = false;
        firstAttack = false;
        parsing.Clear();
    }
}