//using System;
//using UnityEngine;
//using UnityEngine.UI;
//using TMPro;

//using static GameDetails;
//using static GameObjects;
//using static Deck;
//using static Hero;
//using static Enemy;
//using System.Linq;
//using System.Collections;

//public class GameMechanics : MonoBehaviour {
//    #region Variables
//        private static bool ActionOngoing = false;

//        private static int HeroHealthAdjustment = 0, EnemyHealthAdjustment = 0;
//    #endregion

//    #region Methods
//        public void RoundHandler() { 
//            if(!WaitingForNewRound) { 
//                if(!ReadyForAction) { 
//                    if(!HeroTurnSkipped && !HeroSelectedAction) { 
//                        if(UpdateActionTimer()) {
//                            DisplaySelectedCardName(HandToggles.ActiveToggles().FirstOrDefault());
//                        } else { HeroTurnSkipped = true; }
//                    } else { ReadyForAction = true; }
//                } else {
//                    if(!ActionOngoing) { ActionOngoing = true;
//                        ActionHandler();
//                    }
//                }
//            }
//        }

//        public void ActionHandler() {
//            String EnemyAction = null, EnemyHealAction = null, HeroAction = null;

//            EnemyActionAmount = 0; HeroActionAmount = 0; HeroExhaustionAmount = 0;
//            HeroHealthAdjustment = 0; EnemyHealthAdjustment = 0;

//            EnemyActionAmount = CalculateActionValue("Enemy", EnemyAttackDamage, EnemyAttackCriticalGrowth, EnemyAttackCriticalChance, EnemyAttackDiminishingReturn);

//            if(TurnsEnemyStunned == 0) {
//                EnemyTurnSkipped = false;
//                if(EnemyAchievedCritical) {
//                    EnemyAction = $"The {RedText("Enemy")} batters the {GreenText("Hero")}, achieving a {GoldenText("critical")} attack and dealing {RedText(EnemyActionAmount)} damage!";
//                } else {
//                    EnemyAction = $"The {RedText("Enemy")} lashed out at the {GreenText("Hero")}, dealing {RedText(EnemyActionAmount)} damage.";
//                }
//            } else {
//                EnemyTurnSkipped = true;
//                EnemyAction = $"The {RedText("Enemy")} was {PurpleText("stunned")} and could not attack!";
//            }

//            if(!HeroTurnSkipped) {
//                int TurnsToStun = 0;

//                switch(SelectedCard.CardName) {
//                    case "Attack - Slash":
//                        HeroActionAmount = CalculateActionValue("Hero", HeroSlashDamage, HeroSlashCriticalGrowth, HeroSlashCriticalChance, HeroSlashDiminishingReturn);

//                        if(HeroAchievedCritical) {
//                            HeroAction = $"The {GreenText("Hero")} cleaves the {RedText("Enemy")} with their sword, achieving a {GoldenText("critical")} strike against their foe and dealing {GreenText(HeroActionAmount)} damage!";
//                        } else {
//                            HeroAction = $"The {GreenText("Hero")} strikes the {RedText("Enemy")} with their sword, dealing {GreenText(HeroActionAmount)} damage.";
//                        }

//                        EnemyHealthAdjustment -= HeroActionAmount;
//                        if(!EnemyTurnSkipped) { HeroHealthAdjustment -= EnemyActionAmount; }

//                        break;
//                    case "Attack - Smash":
//                        HeroActionAmount = HeroSmashDamage;
//                        TurnsToStun = CalculateActionValue("Hero", 0, HeroSmashCriticalGrowth, HeroSmashStunChance, HeroSmashDiminishingReturn);
//                        TurnsEnemyStunned += TurnsToStun;

//                        if(HeroAchievedCritical) {
//                            HeroAction = $"The {GreenText("Hero")} {PurpleText("staggers")} the {RedText("Enemy")} with a {GoldenText("crushing")} blow, dealing {GreenText(HeroActionAmount)} damage and {PurpleText("stunning")} their foe for {PurpleText(TurnsToStun)} turns!";
//                        } else {
//                            HeroAction = $"The {GreenText("Hero")} smashes the {RedText("Enemy")} with their mace, dealing {GreenText(HeroActionAmount)} damage.";
//                        }

//                        EnemyHealthAdjustment -= HeroActionAmount;
//                        if (!EnemyTurnSkipped) { HeroHealthAdjustment -= EnemyActionAmount; }

//                        break;
//                    case "Special - Parry":
//                        HeroActionAmount = CalculateActionValue("Hero", 0, 1, HeroParryCriticalChance, 100);

//                        if(!EnemyTurnSkipped) {
//                            EnemyAction = null;

//                            if(HeroAchievedCritical) {
//                                HeroActionAmount = EnemyActionAmount * 2;
//                                EnemyActionAmount = 0;
//                                HeroAction = $"The {GreenText("Hero")} strengthens their resolve to achieve a {GoldenText("devastating")} parry against the {RedText("Enemy")}, dealing {GreenText(HeroActionAmount)} damage!";
//                            } else {
//                                HeroActionAmount = EnemyActionAmount;
//                                EnemyActionAmount = 0;
//                                HeroAction = $"The {GreenText("Hero")} parrys the {RedText("Enemy's")} attack, dealing {GreenText(HeroActionAmount)} damage.";
//                            }

//                            EnemyHealthAdjustment -= HeroActionAmount;
//                        } else {
//                            HeroAction = $"The {GreenText("Hero")} braced themselves to parry an {PurpleText("attack that never came...")}";
//                        }

//                        break;
//                    case "Defense - Shield":
//                        HeroActionAmount = CalculateActionValue("Hero", HeroDefenseEffectiveness, HeroDefenseCriticalGrowth, HeroDefenseCriticalChance, HeroDefenseDiminishingReturn);

//                        EnemyAction = null;

//                        if(!EnemyTurnSkipped) { 
//                            if(HeroActionAmount >= EnemyActionAmount) {

//                                if(HeroActionAmount > EnemyActionAmount) {
//                                    TurnsToStun = CalculateActionValue("Hero", 0, 1, HeroDefenseStunChance, 100);
//                                }

//                                if(HeroAchievedCritical) {
//                                    ++TurnsEnemyStunned;
//                                    HeroAction = $"The {GreenText("Hero")} readies their shield at the {GoldenText("perfect")} moment, {SilverText("shielding")} {GreenText(HeroActionAmount)} of {RedText(EnemyActionAmount)} damage and {PurpleText("stunning")} their foe!";
//                                } else {
//                                    HeroAction = $"The {GreenText("Hero")} raises their shield at the right moment, {SilverText("shielding")} {GreenText(HeroActionAmount)} of {RedText(EnemyActionAmount)} damage.";
//                                }
//                            } else {
//                                HeroAction = $"The {GreenText("Hero")} fails to raise their shield in time, {SilverText("shielding")} only {GreenText(HeroActionAmount)} of {RedText(EnemyActionAmount)} damage. The {GreenText("Hero")} takes {RedText(EnemyActionAmount - HeroActionAmount)} damage!";

//                                HeroHealthAdjustment -= (EnemyActionAmount - HeroActionAmount);
//                            }
//                        } else {
//                            HeroAction = $"The {GreenText("Hero")} braced themselves to defend against an {PurpleText("attack that never came...")}";
//                        }

//                        break;
//                    case "Special - Heal":
//                        HeroActionAmount = CalculateActionValue("Hero", HeroHealEffectiveness, HeroHealCriticalGrowth, HeroHealCriticalChance, HeroHealDiminishingReturn);

//                        if(HeroAchievedCritical) {
//                            HeroAction = $"The {GreenText("Hero")} does a {GoldenText("masterful")} job bandaging their wounds, {BlueText("healing")} themselves of {GreenText(HeroActionAmount)} damage!";

//                            if(!EnemyTurnSkipped) {
//                                HeroHealthAdjustment = (HeroActionAmount - EnemyActionAmount);
//                            } else {
//                                HeroHealthAdjustment += HeroActionAmount;
//                            }

//                        } else {
//                            HeroAction = $"The {GreenText("Hero")} bandages their wounds, {BlueText("healing")} themselves of {GreenText(HeroActionAmount)} damage.";
//                        }

//                        break;
//                }
//            } else {
//                if(HeroExhausted) { 
//                    if(ExhaustionDetriment) { 
//                        HeroExhaustionAmount = CalculateActionValue("Hero", HeroExhaustionAmount, HeroExhaustionCriticalGrowth, HeroExhaustionCriticalChance, HeroExhaustionDiminishingReturn);

//                        if(HeroAchievedCritical) {
//                            HeroAction = $"The {GreenText("Hero")} succumbed to their {GoldenText("crippling")} exhaustion, {PurpleText("losing their turn")} and taking {RedText(HeroExhaustionAmount)} damage!";
//                        } else {
//                            HeroAction = $"The {GreenText("Hero")} {PurpleText("staggers")} from great exhaustion, {PurpleText("missing their turn")} and taking {RedText(HeroExhaustionAmount)} damage.";
//                        }

//                        HeroHealthAdjustment -= HeroExhaustionAmount;
//                    } else {
//                        HeroAction = $"The {GreenText("Hero")} {PurpleText("staggers")} from exhaustion, {PurpleText("skipping their turn.")}";
//                    }
//                } else {
//                    HeroAction = $"The {GreenText("Hero")} {PurpleText("hesitated")} in taking action, {PurpleText("missing their turn.")}";
//                }

//                if(!EnemyTurnSkipped) { HeroHealthAdjustment -= EnemyActionAmount; }

//                int EnemyHealAmount = CalculateActionValue("Enemy", EnemyHealEffectiveness, EnemyHealCriticalGrowth, EnemyHealCriticalChance, EnemyHealDiminishingReturn, true);

//                if(EnemyAchievedCriticalHeal) {
//                    EnemyHealAction = $"The {RedText("Enemy")} took advantage of the {GreenText("Hero's")} {PurpleText("lack of action")}, {GoldenText("critically")} {BlueText("healing")} themselves for {RedText(EnemyHealAmount)} damage!";
//                } else {
//                    EnemyHealAction = $"The {RedText("Enemy")} took advantage of the {GreenText("Hero's")} {PurpleText("lack of action")}, {BlueText("healing")} themselves for {RedText(EnemyHealAmount)} damage.";
//                }

//                EnemyHealthAdjustment += EnemyHealAmount;
//            }

//            StartCoroutine(EndOfAction(2.5f, HeroAction, EnemyAction, EnemyHealAction));
//        }
//        public IEnumerator EndOfAction(float DelayInSeconds, String FirstAction = null, String SecondAction = null, String EnemyHealAction = null, int round = 0) {

//            if(FirstAction != null) {
//                CreateHistory(FirstAction);
//                HeroCurrentHealth += HeroHealthAdjustment;
//                yield return new WaitForSeconds(DelayInSeconds);
//            }
        
//            if(EnemyHealAction != null) {
//                CreateHistory(SecondAction);
//                yield return new WaitForSeconds(DelayInSeconds);
//            }

//            if(SecondAction != null) {
//                CreateHistory(EnemyHealAction);
//                EnemyCurrentHealth += EnemyHealthAdjustment;
//            }

//            ActionOngoing = false;

//            if(EnemyDefeated || HeroDefeated || HeroForfeit) {
//                GameOver = true;

//                if(HeroDefeated && EnemyDefeated) {
//                    GameIsDraw = true;
//                }

//                EndOfGame();
//            } else {
//                Prepare();

//                WaitingForNewRound = true;
//                ReadyForAction = false;
//            }
//        }
//        public static void DisplaySelectedCardName(Toggle Active) { 
//            if(Active != null) { 
//                if(Active.name == "Default") {
//                    SelectedCardNameText.SetText("");
//                } else SelectedCardNameText.SetText(Active.gameObject.transform.GetChild(1).name);
//            }
//        }
//        public static void IndicatorHandler() { 
//            if(TurnsEnemyStunned > 0) {
//                EnemyStunIndicator.SetActive(true);
//            } else { EnemyStunIndicator.SetActive(false); }

//            if(HeroTurnSkipped) {
//                TextMeshProUGUI ZoneText = HeroSkippedIndicator.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
//                if(HeroExhausted) ZoneText.SetText("You have three or more exhaustion!\nTurn Skipped!");
//                    else ZoneText.SetText("You ran out of time!\nTurn Skipped!");
//                HeroSkippedIndicator.SetActive(true);
//                Confirm.SetActive(false);
//            } else if(!HeroTurnSkipped) {
//                HeroSkippedIndicator.SetActive(false);
//                if(!WaitingForNewRound) { Confirm.SetActive(true); }
//            }

//            if(WaitingForNewRound) {
//                NextRound.SetActive(true);
//                Confirm.SetActive(false);
//            } else NextRound.SetActive(false);

//            if(EnemyAchievedCritical) {
//                EnemyCriticalIndicator.SetActive(true);
//            } else {
//                EnemyCriticalIndicator.SetActive(false);
//            }

//            if(HeroAchievedCritical) {
//                HeroCritIndicator.SetActive(true);
//            } else {
//                HeroCritIndicator.SetActive(false);
//            }
//        }
//        public static int CalculateActionValue(string origin, int BaseValue, int BaseGrowth, float BaseChance, float DiminishingReturn, bool enemyHeal = false) {
//            bool IsCritical = true;

//            int BarToBeat = 100;
//            int Result = BaseValue;

//            float CriticalChance = (BaseChance / 100);

//            while(IsCritical) {
//                BarToBeat = (int)Math.Round(100 * CriticalChance);
//                if(RNG.Next(101) >= BarToBeat) { 
//                    if(origin.Equals("Hero")) { HeroAchievedCritical = true; }
//                        else { if(enemyHeal) { EnemyAchievedCriticalHeal = true; } else { EnemyAchievedCritical = true; } }

//                    Result += BaseGrowth;
//                    CriticalChance -= (CriticalChance * (DiminishingReturn / 100));
//                } else IsCritical = false;
//            }

//            return Result;
//        }

//        public static void EndOfGame() { 
//            if(GameOver) {
//                var title = EndingPanel.transform.Find("EndTitle").GetComponent<TextMeshProUGUI>();
//                var text = EndingPanel.transform.Find("EndDetails").GetComponent<TextMeshProUGUI>();

//                if(HeroDefeated && !GameIsDraw) {
//                    title.SetText("DEFEATED");
//                    text.SetText("The Hero was defeated by their enemy! Better luck next time...");
//                    title.color = Color.red;
//                }
//                if(EnemyDefeated && !GameIsDraw) {
//                    title.SetText("VICTORY");
//                    text.SetText("The Hero was victorious over their enemy! Onwards, to fame and fortune!");
//                    title.color = Color.green;
//                }
//                if(GameIsDraw) {
//                    title.SetText("DRAW");
//                    text.SetText("The Hero and their enemy lie exhausted, unable to continue their fight. No one wins on this day!");
//                    title.color = Color.yellow;
//                }
//                if(HeroForfeit) {
//                    title.SetText("Forfeit");
//                    text.SetText("The Hero withdraws from the battle, battered and bruised. Their enemy lives to see another day...");
//                    title.color = Color.red;
//                }

//                EndingPanel.SetActive(true);
//            }
//        }
//        public static void Prepare() {
//            if(Hand[0] != null) { DiscardHand(); }

//            if(TurnsEnemyStunned > 0 && EnemyTurnSkipped) { --TurnsEnemyStunned; }

//            TimeRemaining = SelectionTimer;

//            HeroSelectedAction = false;

//            SelectedCard = null;

//            EnemyAchievedCritical = false;
//            EnemyAchievedCriticalHeal = false;
//            HeroAchievedCritical = false;

//            HandToggles.SetAllTogglesOff(true);
//            SelectedCardNameText.SetText("");
//        }
//    #endregion
//}