using System;
using UnityEngine;

public static class Enemy { 
    #region Enemy Health
        [Header("Enemy's Health")]
        public static int EnemyMaxHealth;
        [HideInInspector] public static int EnemyCurrentHealth;
    #endregion
    [Space]
    #region Additional Enemy Details
        [Header("Additional Enemy Details")]
        public static int MaximumEnemyStunTurns;
        [HideInInspector] public static int EnemyActionAmount;
        private static int EnemyStunnedFor = 0;
        [HideInInspector] public static int TurnsEnemyStunned {
            get { return EnemyStunnedFor; } 
            set { 
                EnemyStunnedFor += value; 
                if(EnemyStunnedFor > MaximumEnemyStunTurns) EnemyStunnedFor = MaximumEnemyStunTurns; 
            }
        }
        [HideInInspector] public static bool EnemyAchievedCritical = false;
    #endregion
    [Space]
    #region Enemy Card Values
        [Header("Enemy's Effectiveness")]
        public static int EnemyAttackDamage;
        public static int EnemyHealEffectiveness;
    #endregion
    [Space]
    #region Enemy Critical Values
        [Header("Enemy's Critical Value")]
        public static int EnemyAttackCriticalGrowth;
        public static int EnemyHealCriticalGrowth;
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