using System;
using Unity.VisualScripting;
using UnityEngine;

public static class Hero { 
    #region Hero Health
        [Header("Hero's Health")]
        public static int HeroMaxHealth;
        [HideInInspector] public static int HeroCurrentHealth;
    #endregion
    [Space]
    #region Additional Hero Details
        [Header("Additional Hero Details")]
        [HideInInspector] public static int HeroActionAmount;
        [HideInInspector] public static int HeroExhaustionAmount;
        [HideInInspector] public static bool HeroAchievedCritical = false;
    #endregion
    [Space]
    #region Hero Card Values
        [Header("Hero's Effectiveness")] 
        public static int HeroSlashDamage;
        public static int HeroSmashDamage;
        public static int HeroDefenseEffectiveness;
        public static int HeroHealEffectiveness;
        public static int HeroExhaustionDamage;
    #endregion
    [Space]
    #region Hero Critical Values
        [Header("Hero's Critical Value")]
        public static int HeroSlashCriticalGrowth;
        public static int HeroSmashCriticalGrowth;
        public static int HeroDefenseCriticalGrowth;
        public static int HeroHealCriticalGrowth;
        public static int HeroExhaustionCriticalGrowth;
    #endregion
    [Space]
    #region Hero Chance Values
        [Header("Hero's Chance")]
        [Range(0f, 100f)] public static float HeroSlashCriticalChance;
        [Range(0f, 100f)] public static float HeroSmashStunChance;
        [Range(0f, 100f)] public static float HeroParryCriticalChance;
        [Range(0f, 100f)] public static float HeroDefenseCriticalChance;
        [Range(0f, 100f)] public static float HeroDefenseStunChance;
        [Range(0f, 100f)] public static float HeroHealCriticalChance;
        [Range(0f, 100f)] public static float HeroExhaustionCriticalChance;
    #endregion
    [Space]
    #region Hero Diminishing Returns
        [Header("Hero's Diminishing Return")]
        [Range(0f, 100f)] public static float HeroSlashDiminishingReturn;
        [Range(0f, 100f)] public static float HeroSmashDiminishingReturn;
        [Range(0f, 100f)] public static float HeroDefenseDiminishingReturn;
        [Range(0f, 100f)] public static float HeroHealDiminishingReturn;
        [Range(0f, 100f)] public static float HeroExhaustionDiminishingReturn;
    #endregion

    #region Hero Methods
    public static void ResetHeroHealth() {
        HeroCurrentHealth = HeroMaxHealth;
    }
    #endregion
}