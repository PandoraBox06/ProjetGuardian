using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class ComboSystem : MonoBehaviour
{
    public InputActionReference attackAction;
    public InputActionReference rangeAttackAction;
    public InputActionReference pauseAction;
    public InputActionReference[] comboChain1;
    public InputActionReference[] comboChain2;

    public List<InputAction> parsedInput = new();

    public float attackTime;

    float attackTimer;

    bool canParseInput = false;
    int comboCounter = 0;

    Animator animator;

    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
    }

    private void OnEnable()
    {
        CharacterAnimatorEvents.OnParseInputOn += ParseInputOn;
        CharacterAnimatorEvents.OnParseInputOff += ParseInputOff;
        CharacterAnimatorEvents.OnPauseToParse += AddPauseToParse;
    }

    private void OnDisable()
    {
        CharacterAnimatorEvents.OnParseInputOn -= ParseInputOn;
        CharacterAnimatorEvents.OnParseInputOff -= ParseInputOff;
        CharacterAnimatorEvents.OnPauseToParse -= AddPauseToParse;
    }

    private void Update()
    {
        RegisterInput();     

        if (attackAction.action.WasPerformedThisFrame() && !canParseInput && Time.time > attackTimer)
        {
            MeleeAttack();
        }

        if (parsedInput.Count > 0)
            CheckParsedInput();
    }

    void MeleeAttack()
    {
        comboCounter = 0;
        canParseInput = false;
        comboCounter++;
        animator.Play("Attack" + comboCounter, 0, 0);
        attackTimer = Time.time + attackTime;
    }

    void CheckParsedInput()
    {
        for (int i = 0; i < parsedInput.Count; i++)
        {
            if (comboCounter < comboChain1.Length && parsedInput[i] == comboChain1[comboCounter].action)
            {
                Debug.Log($"Match Combo1");

                comboCounter++;
                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > attackTime && animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
                {
                    animator.Play("Attack" + comboCounter, 0, 0);
                }
                parsedInput.Clear();
                if(comboCounter >= comboChain1.Length)
                {
                    comboCounter = 0;
                }

            }
            else if (comboCounter < comboChain2.Length && parsedInput[i] == comboChain2[comboCounter].action)
            {
                Debug.Log($"Match Combo2");

                comboCounter++;
                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > attackTime && animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
                {
                    animator.Play("AttackB" + comboCounter, 0, 0);
                }
                parsedInput.Clear();
                if (comboCounter >= comboChain2.Length)
                {
                    comboCounter = 0;
                }
            }
            else
            {
                Debug.Log($"No Combo");

                comboCounter = 0;
                parsedInput.Clear();
            }
        }
    }

    void RegisterInput()
    {
        if(canParseInput)
        {
            if(attackAction.action.WasPerformedThisFrame())
            {
                parsedInput.Add(attackAction.action);
                Debug.Log($"Register attack");
            }
            else if (rangeAttackAction.action.WasPerformedThisFrame())
            {
                parsedInput.Add(rangeAttackAction.action);
                Debug.Log($"Register Range Attack");
            }
        }
    }

    void AddPauseToParse()
    {
        if (canParseInput)
        {
            parsedInput.Add(pauseAction.action);
            Debug.Log($"Register pause");
        }
    }

    void ParseInputOn()
    {
        canParseInput = true;
    }
    void ParseInputOff()
    {
        canParseInput = false;
    }
}
