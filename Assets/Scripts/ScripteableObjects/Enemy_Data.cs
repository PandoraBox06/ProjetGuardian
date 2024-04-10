using UnityEngine;


[CreateAssetMenu(fileName = "EnemyData", menuName = "Enemy/Create New Enemy", order = 0)]
public class Enemy_Data : ScriptableObject
{
    public float MaxHealth = 100;
    public float GuardHealth = 50;
    public float WeaponDamage = 30;
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
    [Header("Hp Floor Percentage (X/100%)")]
    [Range(0f, 1f)] public float HighHpPercentLower = .61f;
    [Range(0f, 1f)] public float MidHpPercentUpper = .6f;
    [Range(0f, 1f)] public float MidHpPercentLower = .31f;
    [Range(0f, 1f)] public float LowHpPercentUpper = .3f;
    [Range(0f, 1f)] public float LowHpPercentLower;

    [Header("Percentage For States High HP")] 
    [Range(0f, 1f)] public float HighHpPercentMelee = .6f;
    [Range(0f, 1f)] public float HighHpPercentRange = .3f;
    [Range(0f, 1f)] public float HighHpPercentDodge;

    [Header("Percentage For States Mid HP")] 
    [Range(0f, 1f)] public float MidHpPercentMelee = .3f;
    [Range(0f, 1f)] public float MidHpPercentRange;
    [Range(0f, 1f)] public float MidHpPercentDodge = .6f;

    [Header("Percentage For States Low HP")] 
    [Range(0f, 1f)] public float LowHpPercentMelee;
    [Range(0f, 1f)] public float LowHpPercentRange = .6f;
    [Range(0f, 1f)] public float LowHpPercentDodge = .3f;

    [Header("Time for the current State")] 
    public float RandomTimeForMeleeUpper = 6;
    public float RandomTimeForMeleeLower = 3;
    public float RandomTimeForRangeUpper = 6;
    public float RandomTimeForRangeLower = 3;
    public float RandomTimeForDodgeUpper = 1;
    public float RandomTimeForDodgeLower = .5f;

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
    
    public void SetUpStateManager(out GameObject projectiles, out bool trainingDummyMode, out float idleTime,
        out float meleeAttackRange, out float rangeAttackRange, out float minRangeAttackRange,
        out float projectilesSpeed, out float stunTimer, out float highHpPercentLower, out float midHpPercentUpper,
        out float midHpPercentLower, out float lowHpPercentUpper, out float lowHpPercentLower,
        out float highHpPercentMelee, out float highHpPercentRange, out float highHpPercentDodge,
        out float midHpPercentMelee, out float midHpPercentRange, out float midHpPercentDodge,
        out float lowHpPercentMelee, out float lowHpPercentRange, out float lowHpPercentDodge,
        out float randomTimeForMeleeUpper, out float randomTimeForMeleeLower, out float randomTimeForRangeUpper,
        out float randomTimeForRangeLower, out float randomTimeForDodgeUpper, out float randomTimeForDodgeLower)
    {
        projectiles = Projectiles;
        trainingDummyMode = TrainingDummyMode;
        idleTime = IdleTime;
        meleeAttackRange = MeleeAttackRange;
        rangeAttackRange = RangeAttackRange;
        minRangeAttackRange = MinRangeAttackRange;
        projectilesSpeed = ProjectilesSpeed;
        stunTimer = StunTimer;
        highHpPercentLower = HighHpPercentLower;
        midHpPercentUpper = MidHpPercentUpper;
        midHpPercentLower = MidHpPercentLower;
        lowHpPercentUpper = LowHpPercentUpper;
        lowHpPercentLower = LowHpPercentLower;
        highHpPercentMelee = HighHpPercentMelee;
        highHpPercentRange = HighHpPercentRange;
        highHpPercentDodge = HighHpPercentDodge;
        midHpPercentMelee = MidHpPercentMelee;
        midHpPercentRange = MidHpPercentRange;
        midHpPercentDodge = MidHpPercentDodge;
        lowHpPercentMelee = LowHpPercentMelee;
        lowHpPercentRange = LowHpPercentRange;
        lowHpPercentDodge = LowHpPercentDodge;
        randomTimeForMeleeUpper = RandomTimeForMeleeUpper;
        randomTimeForMeleeLower = RandomTimeForMeleeLower;
        randomTimeForRangeUpper = RandomTimeForRangeUpper;
        randomTimeForRangeLower = RandomTimeForRangeLower;
        randomTimeForDodgeUpper = RandomTimeForDodgeUpper;
        randomTimeForDodgeLower = RandomTimeForDodgeLower;
    }
}
