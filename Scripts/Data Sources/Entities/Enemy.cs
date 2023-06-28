//using System;
//using UnityEngine;

//using static GameObjects;
//using static GameDetails;

//public class Enemy : MonoBehaviour {
//    #region Enemy Health
//        [Header("Enemy's Health")]
//        public static int EnemyMaxHealth;
//        [HideInInspector] public static int EnemyCurrentHealth {
//            get { return EnemyCurrentHealth; }
//            set { 
//                if (value > ((EnemyMaxHealth / 3) * 2)) {
//                    HeroHealthText.color = Color.green;
//                } else if (value <= (EnemyMaxHealth / 3)) {
//                    HeroHealthText.color = Color.red;
//                } else HeroHealthText.color = Color.yellow;

//                if(value <= 0) { EnemyDefeated = true;}

//                EnemyHealthText.SetText("Enemy Health: " + value + " / " + EnemyMaxHealth);
//                EnemyCurrentHealth = value;
//            }
//        }
//    #endregion
//    [Space]
//    #region Additional Enemy Details
//        [Header("Additional Enemy Details")]
//        public static int MaximumEnemyStunTurns;
//        [HideInInspector] public static int EnemyActionAmount;
//        private static int EnemyStunnedFor = 0;
//        [HideInInspector] public static bool EnemyTurnSkipped = false;
//        [HideInInspector] public static int TurnsEnemyStunned {
//            get { return EnemyStunnedFor; } 
//            set { 
//                EnemyStunnedFor += value; 
//                if(EnemyStunnedFor > MaximumEnemyStunTurns) EnemyStunnedFor = MaximumEnemyStunTurns; 
//            }
//        }
//        [HideInInspector] public static bool EnemyAchievedCritical = false;
//        [HideInInspector] public static bool EnemyAchievedCriticalHeal = false;
//    #endregion
//    [Space]
//    #region Enemy Card Values
//        [Header("Enemy's Effectiveness")]
//        public static int EnemyAttackDamage;
//        public static int EnemyHealEffectiveness;
//    #endregion
//    [Space]
//    #region Enemy Critical Values
//        [Header("Enemy's Critical Value")]
//        public static int EnemyAttackCriticalGrowth;
//        public static int EnemyHealCriticalGrowth;
//    #endregion
//    [Space]
//    #region Enemy Chance Values
//        [Header("Enemy's Chance")]
//        [Range(0f, 100f)] public static float EnemyAttackCriticalChance;
//        [Range(0f, 100f)] public static float EnemyHealCriticalChance;
//    #endregion
//    [Space]
//    #region Enemy Diminishing Returns
//        [Header("Enemy's Diminishing Return")]
//        [Range(0f, 100f)] public static float EnemyAttackDiminishingReturn;
//        [Range(0f, 100f)] public static float EnemyHealDiminishingReturn;
//    #endregion

//    #region Enemy Methods
//    public static void ResetEnemyHealth() {
//        EnemyCurrentHealth = EnemyMaxHealth;
//    }
//    #endregion
//}