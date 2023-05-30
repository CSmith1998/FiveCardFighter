using System;
using UnityEngine;

public static class Enemy { 
    #region Enemy Health
        [Header("Enemy's Health")]
        public static int EnemyMaxHealth;
        [HideInInspector] public static int EnemyCurrentHealth;
    #endregion
    [Space]
    #region Enemy Card Values
        [Header("Enemy's Effectiveness")]
        public static int EnemyAttackDamage;
        public static int EnemyHealEffectiveness;
    #endregion
    [Space]
    #region Enemy Chance Values
        [Header("Enemy's Chance")]
        [Range(0f, 100f)] public static float EnemyAttackCriticalChance;
        [Range(0f, 100f)] public static float EnemyHealCriticalChance;
    #endregion
    [Space]
    #region Enemy Diminishing Returns
        [Header("Enemy's Diminishing Return")]
        [Range(0f, 100f)] public static float EnemyAttackDiminishingReturn;
        [Range(0f, 100f)] public static float EnemyHealDiminishingReturn;
    #endregion

    #region Enemy Methods
    public static void ResetEnemyHealth() {
        EnemyCurrentHealth = EnemyMaxHealth;
    }
    #endregion
}