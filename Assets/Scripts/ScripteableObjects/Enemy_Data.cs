using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


[CreateAssetMenu(fileName = "EnemyData", menuName = "Enemy/Create New Enemy", order = 0)]
public class Enemy_Data : ScriptableObject
{
    public float MaxHealth = 100;
    public float GuardHealth = 50;
    public float WeaponDamage = 30;
    public float AttackCooldown = 1;
    public GameObject Projectiles;
    public GameObject VFX_Hit;
    public GameObject VFX_Die;
    [Header("Settings")]
    public bool TrainingDummyMode;
    public float IdleTime = 1;
    public float MeleeAttackRange = 1.5f;
    public float RangeAttackRange = 15;
    public float MinRangeAttackRange = 7;
    public float ProjectilesSpeed = 10;
    public float StunTimer = 1;
    public float WalkingSpeed = 3.5f;
    [Header("Hp Floor Percentage (X/100%)")]
    [Range(0f, 100f)] public float HighHpPercent = 61;
    [Range(0f, 100f)] public float MidHpPercent = 60;
    [Range(0f, 100f)] public float LowHpPercent = 31;

    [Header("Time for the current State")] 
    public float RandomTimeForMeleeUpper = 6;
    public float RandomTimeForMeleeLower = 3;
    public float RandomTimeForRangeUpper = 6;
    public float RandomTimeForRangeLower = 3;
    public float RandomTimeForDodgeUpper = 4;
    public float RandomTimeForDodgeLower = 2;

    public List<RandomBehaviour> RandomBehaviours = new List<RandomBehaviour>();
    public List<RandomBehaviour> FullLifeBehaviours;
    public List<RandomBehaviour> MidLifeBehaviours;
    public List<RandomBehaviour> LowLifeBehaviours;
    [Serializable] public class RandomBehaviour
    {
        public EnemyState State;
        public int Weight;
    }
    
    [NonSerialized] private int _totalweight = -1;

    private int TotalWeight
    {
        get
        {
            if (_totalweight == -1)
            {
                CaculateTotalWeight();
            }

            return _totalweight;
        }
    }
    
    private void CaculateTotalWeight()
    {
        _totalweight = 0;
        foreach (var t in RandomBehaviours)
        {
            _totalweight += t.Weight;
        }
    }

    public EnemyState GetBehaviour()
    {
        int roll = Random.Range(0, TotalWeight);
        foreach (var behaviour in RandomBehaviours)
        {
            roll -= behaviour.Weight;
            if (roll < 0)
            {
                return behaviour.State;
            }
        }

        return RandomBehaviours[0].State;
    }
    
    public void SetUpEnemy(out float maxHealth, out GameObject vfx_Hit, out GameObject vfx_Die, out float guardHealth)
    {
        maxHealth = MaxHealth;
        vfx_Hit = VFX_Hit;
        vfx_Die = VFX_Die;
        guardHealth = GuardHealth;
    }
    
    public void SetUpWeapon(out float weaponDamage)
    {
        weaponDamage = WeaponDamage;
    }
}
