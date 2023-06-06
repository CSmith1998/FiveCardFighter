using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

using static GameDetails;
using static GameObjects;
using static Hero;
using static Enemy;
using System.Linq;

public static class GameMechanics {
    private static bool ActionOngoing = false;

    public static void RoundHandler() { 
        if(!WaitingForNewRound) { 
            if(!ReadyForAction) { 
                if(!HeroTurnSkipped && !SelectionMade) { 
                    if(UpdateActionTimer()) {
                        DisplaySelectedCardName(HandToggles.ActiveToggles().FirstOrDefault());
                    } else { HeroTurnSkipped = true; }
                } else { ReadyForAction = true; }
            } else {
                if(!ActionOngoing) { ActionOngoing = true;
                    if(!HeroTurnSkipped) {
                        int TurnsToStun = 0;

                        EnemyActionAmount = CalculateActionValue("Enemy", EnemyAttackDamage, EnemyAttackCriticalGrowth, EnemyAttackCriticalChance, EnemyAttackDiminishingReturn);

                        switch(SelectedCard.CardName) {
                            case "Attack - Slash":
                                HeroActionAmount = CalculateActionValue("Hero", HeroSlashDamage, HeroSlashCriticalGrowth, HeroSlashCriticalChance, HeroSlashDiminishingReturn);
                                break;
                            case "Attack - Smash":
                                HeroActionAmount = HeroSmashDamage;
                                TurnsToStun += CalculateActionValue("Hero", 0, HeroSmashCriticalGrowth, HeroSmashStunChance, HeroSmashDiminishingReturn);
                                TurnsEnemyStunned += TurnsToStun;
                                break;
                            case "Special - Parry":
                                HeroActionAmount = CalculateActionValue("Hero", 0, 1, HeroParryCriticalChance, 100);
                                if(HeroActionAmount > 0) {
                                    HeroActionAmount = EnemyActionAmount * 2;
                                    EnemyActionAmount = 0;
                                } else {
                                    HeroActionAmount = EnemyActionAmount;
                                    EnemyActionAmount = 0;
                                }
                                break;
                            case "Defense - Shield":
                                HeroActionAmount = CalculateActionValue("Hero", HeroDefenseEffectiveness, HeroDefenseCriticalGrowth, HeroDefenseCriticalChance, HeroDefenseDiminishingReturn);
                                if(HeroActionAmount > EnemyActionAmount) {
                                    TurnsToStun = CalculateActionValue("Hero", 0, 1, HeroDefenseStunChance, 100);
                                    if(TurnsToStun > 0) ++TurnsEnemyStunned;
                                }
                                break;
                            case "Special - Heal":
                                HeroActionAmount = CalculateActionValue("Hero", HeroHealEffectiveness, HeroHealCriticalGrowth, HeroHealCriticalChance, HeroHealDiminishingReturn);
                                break;
                            case "Detriment - Exhaustion":
                                HeroExhaustionAmount = CalculateActionValue("Hero", HeroExhaustionAmount, HeroExhaustionCriticalGrowth, HeroExhaustionCriticalChance, HeroExhaustionDiminishingReturn);
                                break;
                        }
                    } else {
                        int EnemyAttackAmount = CalculateActionValue("Enemy", EnemyAttackDamage, EnemyAttackCriticalGrowth, EnemyAttackCriticalChance, EnemyAttackDiminishingReturn);
                        int EnemyHealAmount = CalculateActionValue("Enemy", EnemyHealEffectiveness, EnemyHealCriticalGrowth, EnemyHealCriticalChance, EnemyHealDiminishingReturn);
                    }
                    ActionOngoing = false;
                }
            }
        }
    }
    public static void DisplaySelectedCardName(Toggle Active) { 
        if(Active != null) { 
            if(Active.name == "Default") {
                SelectedCardNameText.SetText("");
            } else SelectedCardNameText.SetText(Active.gameObject.transform.GetChild(1).name);
        }
    }
    public static void IndicatorHandler() { 
        if(TurnsEnemyStunned > 0) {
            EnemyStunIndicator.SetActive(true);
        } else { EnemyStunIndicator.SetActive(false); }

        if(HeroTurnSkipped) {
            TextMeshProUGUI ZoneText = HeroSkippedIndicator.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            if(HeroExhausted) ZoneText.SetText("You have three or more exhaustion!\nTurn Skipped!");
                else ZoneText.SetText("You ran out of time!\nTurn Skipped!");
            HeroSkippedIndicator.SetActive(true);
            Confirm.SetActive(false);
        } else if(!HeroTurnSkipped) {
            HeroSkippedIndicator.SetActive(false);
            if(!WaitingForNewRound) { Confirm.SetActive(true); }
        }

        if(WaitingForNewRound) {
            NextRound.SetActive(true);
            Confirm.SetActive(false);
        } else NextRound.SetActive(false);

        if(EnemyAchievedCritical) {
            EnemyCriticalIndicator.SetActive(true);
        } else {
            EnemyCriticalIndicator.SetActive(false);
        }

        if(HeroAchievedCritical) {
            HeroCritIndicator.SetActive(true);
        } else {
            HeroCritIndicator.SetActive(false);
        }
    }
    public static int CalculateActionValue(string origin, int BaseValue, int BaseGrowth, float BaseChance, float DiminishingReturn) {
        bool IsCritical = true;

        int BarToBeat = 100;
        int Result = BaseValue;

        float CriticalChance = (BaseChance / 100);

        while(IsCritical) {
            BarToBeat = (int)Math.Round(100 * CriticalChance);
            if(RNG.Next(101) >= BarToBeat) { 
                if(origin.Equals("Hero")) { HeroAchievedCritical = true; }
                    else { EnemyAchievedCritical = true; }

                Result += BaseGrowth;
                CriticalChance -= (CriticalChance * (DiminishingReturn / 100));
            } else IsCritical = false;
        }

        return Result;
    }
}