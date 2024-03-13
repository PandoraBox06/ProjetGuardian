using UnityEngine;


[CreateAssetMenu(fileName = "EnemyData", menuName = "Enemy/Create New Enemy", order = 0)]
public class Enemy_Data : ScriptableObject
{
    public float MaxHealth = 100;
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

    public void SetUpEnemy(float maxHealth, GameObject vfx_Hit, GameObject vfx_Die)
    {
        maxHealth = MaxHealth;
        vfx_Hit = VFX_Hit;
        vfx_Die = VFX_Die;
    }
    
    public void SetUpWeapon(float weaponDamage)
    {
        weaponDamage = WeaponDamage;
    }
    
    public void SetUpStateManager(GameObject projectiles, bool trainingDummyMode, float idleTime, float meleeAttackRange, float rangeAttackRange,
        float minRangeAttackRange, float projectilesSpeed, float stunTimer, float highHpPercentLower,
        float midHpPercentUpper, float midHpPercentLower, float lowHpPercentUpper, float lowHpPercentLower,
        float highHpPercentMelee, float highHpPercentRange, float highHpPercentDodge, float midHpPercentMelee,
        float midHpPercentRange, float midHpPercentDodge, float lowHpPercentMelee, float lowHpPercentRange,
        float lowHpPercentDodge, float randomTimeForMeleeUpper, float randomTimeForMeleeLower,
        float randomTimeForRangeUpper, float randomTimeForRangeLower, float randomTimeForDodgeUpper,
        float randomTimeForDodgeLower)
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
