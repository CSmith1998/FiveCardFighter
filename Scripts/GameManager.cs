using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

using Random = System.Random;

public class GameManager : MonoBehaviour {
    public Dictionary<string, Card> AvailableCards;
    private void Start() {
        HandToggles = HandToggleGroup.GetComponent<ToggleGroup>();
        AvailableCards = new Dictionary<string, Card>() {
                { "Attack - Slash", new Card("Attack - Slash", SlashSprite) },
                { "Attack - Smash", new Card("Attack - Smash", SmashSprite) },
                { "Special - Parry", new Card("Special - Parry", ParrySprite) },
                { "Defense - Shield", new Card("Defense - Shield", DefenseSprite) },
                { "Special - Heal", new Card("Special - Heal", HealingSprite) },
                { "Detriment - Exhaustion", new Card("Detriment - Exhaustion", ExhaustionSprite) }
        };
    }

    private void Update() {
        if(GameField.activeInHierarchy && !GameOver && !ConfirmationMenu.activeInHierarchy) {
            if(!GamePaused) { CheckForPause();
                IndicatorHandler();
                RoundHandler();
            } else CheckForUnpause();
        }
    }

    #region Button Methods
        public void ClickedHelp() {
            AboutMenu.SetActive(true);
        }
        public void ReturnToMenu() { 
            if(GameField.activeInHierarchy) {
                GameField.SetActive(false);
            }
            if(EndingPanel.activeInHierarchy) {
                EndingPanel.SetActive(false);
            }
            MainMenu.SetActive(true);
            Prepare();
            
            ActionOngoing = false;
            ReadyForAction = false;
            HeroTurnSkipped = false;
            WaitingForNewRound = false;
            GameOver = false;
            HeroForfeit = false;
            HeroDefeated = false;
            EnemyDefeated = false;
        }
        public void ClickedStartGame() {
            if(MainMenu.activeInHierarchy) {
                MainMenu.SetActive(false);
                GamePaused = true;
                GameOver = false;
                HeroForfeit = false;
                HeroDefeated = false;
                EnemyDefeated = false;
            }

            if(Library != null) Library.Clear();
            if(Hand != null) ClearHand();
            if(Graveyard != null) Graveyard.Clear();
            if(HistoryLog != null) ResetHistory();

            Round = 0;

            ResetEnemyHealth();
            ResetHeroHealth();

            HandleHealth();

            GameField.SetActive(true);
            BeginPanel.SetActive(true);

            InitializeLibrary();
        }
        public void ClickedBegin() {
            BeginPanel.SetActive(false);
            TimeRemaining = SelectionTimer;
            GamePaused = false;

            HandToggleGroup.transform.GetChild(0).GetComponent<Toggle>().isOn = true;

            Draw();
        }
        public void ClickedSubmit() {
            //Debug.Log($"Hero Selected Action = {HeroSelectedAction} :: Before");
            var selection = HandToggles.ActiveToggles().First().gameObject.transform.GetChild(1).name;
            //Debug.Log($"Selected Card Name: {selection}");
            if(selection != "Default" && selection != null) {
                SelectedCard = selection;
                HeroSelectedAction = true;
                //Debug.Log($"Hero Selected Action =  {HeroSelectedAction} :: After");
            }
        }
        public void ClickedNextTurn() {
            if(!GameOver) {
                Prepare();
                Draw();
                ActionOngoing = false;
                ReadyForAction = false;
                WaitingForNewRound = false;
            }
        }
        public void ClickedForfeit() {
            ConfirmationMenu.SetActive(true);
            Time.timeScale = 0;
        }
        
        public void ClickedYes() { 
            ConfirmationMenu.SetActive(false);
            GameOver = true;
            HeroForfeit = true;
            EndOfGame();
        }
        public void ClickedNo() { 
            ConfirmationMenu.SetActive(false);
            Time.timeScale = 1;
        }
        public void ClickedResume() {
            if(PauseMenu.activeInHierarchy && GamePaused) {
                Unpause();
            }
        }

        #region Exit Buttons
            public void ClickedExit() {
                AboutMenu.SetActive(false);
            }
            public void ClickedQuit() {
                Application.Quit();
            }
        #endregion
    #endregion

    #region Graphics
        #region Card Sprites
            [Header("Card Images")]
            public Sprite SlashSprite;
            public Sprite SmashSprite;
            public Sprite ParrySprite;
            public Sprite DefenseSprite;
            public Sprite HealingSprite;
            public Sprite ExhaustionSprite;
        #endregion
    #endregion

    #region Deck
        #region Hero's Deck
            [HideInInspector] public Stack<Card> Library = new Stack<Card>();
            #region Related Methods
                public void InitializeLibrary() {
                    List<Card> temp = new List<Card>();
                    temp.AddRange(Enumerable.Repeat(AvailableCards.GetValueOrDefault("Attack - Slash"), 7));
                    temp.AddRange(Enumerable.Repeat(AvailableCards.GetValueOrDefault("Attack - Smash"), 5));
                    temp.AddRange(Enumerable.Repeat(AvailableCards.GetValueOrDefault("Special - Parry"), 2));
                    temp.AddRange(Enumerable.Repeat(AvailableCards.GetValueOrDefault("Defense - Shield"), 5));
                    temp.AddRange(Enumerable.Repeat(AvailableCards.GetValueOrDefault("Special - Heal"), 4));
                    temp.AddRange(Enumerable.Repeat(AvailableCards.GetValueOrDefault("Detriment - Exhaustion"), 7));

                    temp = Shuffle(temp);

                    foreach(Card item in temp) {
                        Library.Push(item);
                    }
                }
                public void FillLibrary(List<Card> deck) { 
                    foreach(Card card in deck) { 
                        Library.Push(card);
                    }
                }
            #endregion
        #endregion

        #region Hero's Graveyard
            [HideInInspector] public List<Card> Graveyard = new List<Card>();
            [HideInInspector] public int CardsToLose = 0;
            public void DiscardHand() {
                string UsedCard = null;
                bool HealDeleted = false;

                if(SelectedCard != null) { UsedCard = SelectedCard; }

                if(ExhaustionDetriment) {
                    List<Card> ReturnToDeck = new List<Card>();
            
                    if(Library.Count() >= CardsToLose) { 
                        for(int i = 0; i < CardsToLose; ++i) {
                            Card card = Library.Peek();
                            while(card.CardName == "Detriment - Exhaustion") { 
                                ReturnToDeck.Add(Library.Pop());
                                card = Library.Peek();
                            }
                            Library.Pop();
                        }
                        foreach(Card item in ReturnToDeck) {
                            Library.Push(item);
                        }
                    } else { 
                        for(int i = 0; i < CardsToLose; ++i) {
                            int rnd = RNG.Next(0, Graveyard.Count());
                            var card = Graveyard[rnd];
                            while(card.CardName == "Detriment - Exhaustion") {
                                rnd = RNG.Next(0, Graveyard.Count());
                                card = Graveyard[rnd];
                            }
                            Graveyard.RemoveAt(rnd);
                        }
                    }
                }

                if (UsedCard == "Special - Heal") {
                    for (int i = 0; i < Hand.Length; ++i) {
                        if (Hand[i].CardName == "Special - Heal" && !HealDeleted) {
                            Hand[i] = null;
                            HealDeleted = true;
                        } else Graveyard.Add(Hand[i]);
                    }
                } else { 
                    for(int i = 0; i < Hand.Length; ++i) {
                        Graveyard.Add(Hand[i]);
                        Hand[i] = null;
                    }
                }
            }
        #endregion

        #region Hero's Hand
            [HideInInspector] public Card[] Hand = new Card[5];
            public void Draw() {
                int exhaustion = 0;

                for(int i = 0; i < 5; ++i) {
                    Transform card = HandToggleGroup.transform.GetChild(i + 1);
                    Toggle cardToggle = card.GetComponent<Toggle>();

                    if(Library.Count == 0) {
                        FillLibrary(Shuffle(Graveyard));
                        Graveyard.Clear();
                    }

                    Hand[i] = Library.Pop();

                    if(Hand[i].CardName == "Detriment - Exhaustion") {
                        ++exhaustion;
                        cardToggle.interactable = false;
                    } else cardToggle.interactable = true;

                    card.Find("Background").Find("Card Image").GetComponent<Image>().sprite = Hand[i].CardFace;
                    card.GetChild(1).name = Hand[i].CardName;
                }

                if(exhaustion >= 3) {
                    HeroTurnSkipped = true;
                    HeroExhausted = true;

                    DisableHand();

                    if(exhaustion == 5) {
                        ExhaustionDetriment = true;
                        CardsToLose = RNG.Next(0, 5);
                    } else { 
                        ExhaustionDetriment = false;
                        CardsToLose = 0;
                    }

                } else {
                    HeroTurnSkipped = false;
                    HeroExhausted = false;
                }
            }
            public void DisableHand() { 
                for(int i = 0; i < Hand.Count(); ++i) {
                    HandToggleGroup.transform.GetChild(i + 1).GetComponent<Toggle>().interactable = false;
                }
            }
            public void ClearHand() { 
                for(int i = 0; i < Hand.Count(); ++i) {
                    Hand[i] = null;
                }
            }
        #endregion

        #region Extension Methods
            private List<T> Shuffle<T>(List<T> list) {
                Random rng = new Random();
                int count = list.Count;

                for (int i = count - 1; i > 0; --i) {
                    int k = rng.Next(i + 1);
                    T value = list[k];
                    list[k] = list[i];
                    list[i] = value;
                }

                return list;
            }
        #endregion
    #endregion

    #region GameMechanics
        #region Variables
            private bool ActionOngoing = false;

            private int HeroHealthAdjustment = 0, EnemyHealthAdjustment_Damage = 0, EnemyHealthAdjustment_Heal = 0;
            private int Round = 0;
        #endregion

        #region Methods
            public void RoundHandler() { 
                if(!WaitingForNewRound) { 
                    //Debug.Log("Waiting For New Round");
                    if(!ReadyForAction) { 
                        //Debug.Log("Not Ready For Action");
                        if(!HeroTurnSkipped && !HeroSelectedAction) { 
                            //Debug.Log("Not skipped and not selected");
                            if(UpdateActionTimer()) {
                                DisplaySelectedCardName(HandToggles.ActiveToggles().First());
                            } else { HeroTurnSkipped = true; /*Debug.Log("Turn Skipped");*/ }
                        } else { ReadyForAction = true; /*Debug.Log("Ready for Action");*/ }
                    } else {
                        //Debug.Log("Action not ongoing");
                        if(!ActionOngoing) { ActionOngoing = true;
                            //Debug.Log("Action ongoing and approaching handler.");
                            ActionHandler();
                        }
                    }
                }
            }

            public void ActionHandler() {
                //Debug.Log("Entered handler");
                String EnemyAction = null, EnemyHealAction = null, HeroAction = null;
                int TurnsToStun = 0;

                EnemyActionAmount = 0; HeroActionAmount = 0; HeroExhaustionAmount = 0;
                HeroHealthAdjustment = 0; EnemyHealthAdjustment_Damage = 0; EnemyHealthAdjustment_Heal = 0;

                if(EnemyStunnedFor == 0) {
                    //Debug.Log("Checkpoint 1");

                    EnemyActionAmount = CalculateActionValue("Enemy", EnemyAttackDamage, EnemyAttackCriticalGrowth, EnemyAttackCriticalChance, EnemyAttackDiminishingReturn);

                    EnemyTurnSkipped = false;
                    if(EnemyAchievedCritical) {
                        EnemyAction = $"The {RedText("Enemy")} batters the {GreenText("Hero")}, achieving a {GoldenText("critical")} attack and dealing {RedText(EnemyActionAmount)} damage!";
                    } else {
                        EnemyAction = $"The {RedText("Enemy")} lashed out at the {GreenText("Hero")}, dealing {RedText(EnemyActionAmount)} damage.";
                    }
                } else {
                    EnemyTurnSkipped = true;
                    EnemyAction = $"The {RedText("Enemy")} was {PurpleText("stunned")} and could not attack!";
                }

                if(!HeroTurnSkipped) {
                    //Debug.Log("Checkpoint 2");

                    switch(SelectedCard) {
                        case "Attack - Slash":
                            HeroActionAmount = CalculateActionValue("Hero", HeroSlashDamage, HeroSlashCriticalGrowth, HeroSlashCriticalChance, HeroSlashDiminishingReturn);

                            if(HeroAchievedCritical) {
                                HeroAction = $"The {GreenText("Hero")} cleaves the {RedText("Enemy")} with their sword, achieving a {GoldenText("critical")} strike against their foe and dealing {GreenText(HeroActionAmount)} damage!";
                            } else {
                                HeroAction = $"The {GreenText("Hero")} strikes the {RedText("Enemy")} with their sword, dealing {GreenText(HeroActionAmount)} damage.";
                            }

                            EnemyHealthAdjustment_Damage += HeroActionAmount;
                            if(!EnemyTurnSkipped) { HeroHealthAdjustment -= EnemyActionAmount; }

                            break;
                        case "Attack - Smash":
                            HeroActionAmount = HeroSmashDamage;
                            TurnsToStun = CalculateActionValue("Hero", 0, HeroSmashCriticalGrowth, HeroSmashStunChance, HeroSmashDiminishingReturn);
                        
                            if((EnemyStunnedFor + TurnsToStun) > MaximumEnemyStunTurns) { 
                                TurnsToStun = (MaximumEnemyStunTurns - EnemyStunnedFor);
                            }

                            if(HeroAchievedCritical) {
                                HeroAction = $"The {GreenText("Hero")} {PurpleText("staggers")} the {RedText("Enemy")} with a {GoldenText("crushing")} blow, dealing {GreenText(HeroActionAmount)} damage and {PurpleText("stunning")} their foe for {PurpleText(TurnsToStun)} turns!";
                            } else {
                                HeroAction = $"The {GreenText("Hero")} smashes the {RedText("Enemy")} with their mace, dealing {GreenText(HeroActionAmount)} damage.";
                            }

                            EnemyHealthAdjustment_Damage += HeroActionAmount;
                            if (!EnemyTurnSkipped) { HeroHealthAdjustment -= EnemyActionAmount; }

                            break;
                        case "Special - Parry":
                            HeroActionAmount = CalculateActionValue("Hero", 0, 1, HeroParryCriticalChance, 100);

                            if(!EnemyTurnSkipped) {
                                EnemyAction = null;

                                if(HeroAchievedCritical) {
                                    HeroActionAmount = EnemyActionAmount * 2;
                                    EnemyActionAmount = 0;
                                    HeroAction = $"The {GreenText("Hero")} strengthens their resolve to achieve a {GoldenText("devastating")} parry against the {RedText("Enemy")}, dealing {GreenText(HeroActionAmount)} damage!";
                                } else {
                                    HeroActionAmount = EnemyActionAmount;
                                    EnemyActionAmount = 0;
                                    HeroAction = $"The {GreenText("Hero")} parrys the {RedText("Enemy's")} attack, dealing {GreenText(HeroActionAmount)} damage.";
                                }

                                EnemyHealthAdjustment_Damage += HeroActionAmount;
                            } else {
                                HeroAction = $"The {GreenText("Hero")} braced themselves to parry an {PurpleText("attack that never came...")}";
                            }

                            break;
                        case "Defense - Shield":
                            HeroActionAmount = CalculateActionValue("Hero", HeroDefenseEffectiveness, HeroDefenseCriticalGrowth, HeroDefenseCriticalChance, HeroDefenseDiminishingReturn);
                            bool CriticalDefense = false;

                            if(HeroAchievedCritical) { 
                                CriticalDefense = true;
                                HeroAchievedCritical = false;
                            }

                            EnemyAction = null;

                            if(!EnemyTurnSkipped) { 
                                if(HeroActionAmount >= EnemyActionAmount) {

                                    if(HeroActionAmount > EnemyActionAmount) {
                                        TurnsToStun = CalculateActionValue("Hero", 0, 1, HeroDefenseStunChance, 100);
                                        if(TurnsToStun > 1) { TurnsToStun = 1; }
                                    }

                                    if(HeroAchievedCritical) {
                                        HeroAction = $"The {GreenText("Hero")} readies their shield at the {GoldenText("perfect")} moment, {SilverText("shielding")} {GreenText(HeroActionAmount)} of {RedText(EnemyActionAmount)} damage and {PurpleText("stunning")} their foe!";
                                    } else if(!HeroAchievedCritical && CriticalDefense) {
                                        HeroAction = $"The {GreenText("Hero")} readies their shield at the {GoldenText("opportune")} moment, {SilverText("shielding")} {GreenText(HeroActionAmount)} of {RedText(EnemyActionAmount)} damage!";
                                    } else { 
                                        HeroAction = $"The {GreenText("Hero")} raises their shield at the right moment, {SilverText("shielding")} {GreenText(HeroActionAmount)} of {RedText(EnemyActionAmount)} damage.";
                                    }
                                } else {
                                    HeroAction = $"The {GreenText("Hero")} fails to raise their shield in time, {SilverText("shielding")} only {GreenText(HeroActionAmount)} of {RedText(EnemyActionAmount)} damage. The {GreenText("Hero")} takes {RedText(EnemyActionAmount - HeroActionAmount)} damage!";

                                    HeroHealthAdjustment -= (EnemyActionAmount - HeroActionAmount);
                                }
                            } else {
                                HeroAction = $"The {GreenText("Hero")} braced themselves to defend against an {PurpleText("attack that never came...")}";
                            }

                            break;
                        case "Special - Heal":
                            HeroActionAmount = CalculateActionValue("Hero", HeroHealEffectiveness, HeroHealCriticalGrowth, HeroHealCriticalChance, HeroHealDiminishingReturn);

                            if(HeroAchievedCritical) {
                                HeroAction = $"The {GreenText("Hero")} does a {GoldenText("masterful")} job bandaging their wounds, {BlueText("healing")} themselves of {GreenText(HeroActionAmount)} damage!";
                            } else {
                                HeroAction = $"The {GreenText("Hero")} bandages their wounds, {BlueText("healing")} themselves of {GreenText(HeroActionAmount)} damage.";
                            }

                            if(!EnemyTurnSkipped) {
                                HeroHealthAdjustment = (HeroActionAmount - EnemyActionAmount);
                            } else {
                                HeroHealthAdjustment += HeroActionAmount;
                            }

                            break;
                    }
                } else {
                    if(HeroExhausted) { 
                        if(ExhaustionDetriment) { 
                            HeroExhaustionAmount = CalculateActionValue("Hero", HeroExhaustionAmount, HeroExhaustionCriticalGrowth, HeroExhaustionCriticalChance, HeroExhaustionDiminishingReturn);

                            if(HeroAchievedCritical) {
                                HeroAction = $"The {GreenText("Hero")} succumbed to their {GoldenText("crippling")} exhaustion, {PurpleText("losing their turn")} and taking {RedText(HeroExhaustionAmount)} damage!";
                            } else {
                                HeroAction = $"The {GreenText("Hero")} {PurpleText("staggers")} from great exhaustion, {PurpleText("missing their turn")} and taking {RedText(HeroExhaustionAmount)} damage.";
                            }

                            HeroHealthAdjustment -= HeroExhaustionAmount;
                        } else {
                            HeroAction = $"The {GreenText("Hero")} {PurpleText("staggers")} from exhaustion, {PurpleText("skipping their turn.")}";
                        }
                    } else {
                        HeroAction = $"The {GreenText("Hero")} {PurpleText("hesitated")} in taking action, {PurpleText("missing their turn.")}";
                    }

                    if(!EnemyTurnSkipped) { HeroHealthAdjustment -= EnemyActionAmount; }

                    int EnemyHealAmount = CalculateActionValue("Enemy", EnemyHealEffectiveness, EnemyHealCriticalGrowth, EnemyHealCriticalChance, EnemyHealDiminishingReturn, true);

                    if(EnemyAchievedCriticalHeal) {
                        EnemyHealAction = $"The {RedText("Enemy")} took advantage of the {GreenText("Hero's")} {PurpleText("lack of action")}, {GoldenText("critically")} {BlueText("healing")} themselves for {RedText(EnemyHealAmount)} damage!";
                    } else {
                        EnemyHealAction = $"The {RedText("Enemy")} took advantage of the {GreenText("Hero's")} {PurpleText("lack of action")}, {BlueText("healing")} themselves for {RedText(EnemyHealAmount)} damage.";
                    }

                    EnemyHealthAdjustment_Heal += EnemyHealAmount;
                    if((EnemyCurrentHealth + EnemyHealthAdjustment_Heal) > EnemyMaxHealth) { 
                        EnemyHealthAdjustment_Heal = (EnemyMaxHealth - EnemyCurrentHealth);
                    }
                }
                //Debug.Log("Checkpoint 3");
                if(HeroAction == null) { 
                    HeroAction = "";
                }
                if(EnemyAction == null) { 
                    EnemyAction = "";
                }
                if(EnemyHealAction == null) { 
                    EnemyHealAction = "";
                }
                StartCoroutine(EndOfAction(1f, TurnsToStun, HeroAction, EnemyAction, EnemyHealAction));
                //EndOfAction(2.5f, TurnsToStun, HeroAction, EnemyAction, EnemyHealAction);
            }
            public IEnumerator EndOfAction(float DelayInSeconds, int TurnsToStun = 0, String FirstAction = null, String SecondAction = null, String EnemyHealAction = null, int round = 0) {
                CreateSpacer();

                EnemyStunnedFor += TurnsToStun;
                if(EnemyStunnedFor > MaximumEnemyStunTurns) { EnemyStunnedFor = MaximumEnemyStunTurns; }
                
                //Debug.Log("Entered Coroutine");
                //Debug.Log($"Cards in Library = {Library.Count} :: Before");

                if(FirstAction != null && FirstAction != "") {
                    CreateHistory(FirstAction);
                    EnemyCurrentHealth -= EnemyHealthAdjustment_Damage;
                    yield return new WaitForSeconds(DelayInSeconds);
                }
        
                if(EnemyHealAction != null && EnemyHealAction != "") {
                    CreateHistory(EnemyHealAction);
                    EnemyCurrentHealth += EnemyHealthAdjustment_Heal;
                    yield return new WaitForSeconds(DelayInSeconds);
                }
        
                if(SecondAction != null && SecondAction != "") {
                    CreateHistory(SecondAction);
                }

                HeroCurrentHealth += HeroHealthAdjustment;

                HandleHealth();

                FinalizeHistory();

                if(EnemyDefeated || HeroDefeated || HeroForfeit) {
                    if(HeroDefeated && EnemyDefeated) {
                        GameIsDraw = true;
                    }
                    EndOfGame();
                } else {
                    //Prepare();
                    //Debug.Log($"Cards in Library = {Library.Count} :: After");
                    WaitingForNewRound = true;
                }
            }
            public void DisplaySelectedCardName(Toggle Active) { 
                if(Active != null) { 
                    var selected = Active.gameObject.transform.GetChild(1).name;
                    if(selected == "Default") {
                        SelectedCardNameText.SetText("");
                    } else SelectedCardNameText.SetText(selected);
                }
            }
            public void IndicatorHandler() { 
                if(EnemyStunnedFor > 0) {
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
            public int CalculateActionValue(string origin, int BaseValue, int BaseGrowth, float BaseChance, float DiminishingReturn, bool enemyHeal = false) {
                bool IsCritical = true;

                int BarToBeat = 100;
                int Result = BaseValue;

                float CriticalChance = BaseChance;
                //Debug.Log("Critical Loop :: Before");
                while(IsCritical) {
                    //Debug.Log("Entered Critical Roll");
                    BarToBeat = (int)Math.Round(100 - CriticalChance);
                    //Debug.Log($"Bar To Beat: {BarToBeat}");
                    if(RNG.Next(101) >= BarToBeat) { 
                        if(origin.Equals("Hero")) { HeroAchievedCritical = true; }
                            else { if(enemyHeal) { EnemyAchievedCriticalHeal = true; } else { EnemyAchievedCritical = true; } }

                        Result += BaseGrowth;
                        CriticalChance -= (CriticalChance * (DiminishingReturn / 100));
                    } else IsCritical = false;
                }

                return Result;
            }

            public void EndOfGame() { 
                GameOver = true;

                var title = EndingPanel.transform.Find("EndTitle").GetComponent<TextMeshProUGUI>();
                var text = EndingPanel.transform.Find("EndDetails").GetComponent<TextMeshProUGUI>();

                if(HeroDefeated && !GameIsDraw) {
                    title.SetText("DEFEATED");
                    text.SetText("The Hero was defeated by their enemy! Better luck next time...");
                    title.color = Color.red;
                }
                if(EnemyDefeated && !GameIsDraw) {
                    title.SetText("VICTORY");
                    text.SetText("The Hero was victorious over their enemy! Onwards, to fame and fortune!");
                    title.color = Color.green;
                }
                if(GameIsDraw) {
                    title.SetText("DRAW");
                    text.SetText("The Hero and their enemy lie exhausted, unable to continue their fight. No one wins on this day!");
                    title.color = Color.yellow;
                }
                if(HeroForfeit) {
                    title.SetText("Forfeit");
                    text.SetText("The Hero withdraws from the battle, battered and bruised. Their enemy lives to see another day...");
                    title.color = Color.red;
                }

                EndingPanel.SetActive(true);
            }
            public void Prepare() {
                //Debug.Log($"Entered Prepare.");
                
                if(Hand[0] != null) { DiscardHand(); /*Debug.Log($"Discarded Hand.");*/ }

                if(EnemyStunnedFor > 0 && EnemyTurnSkipped) { 
                    Debug.Log($"Enemy Skip Counter {EnemyStunnedFor} :: BEFORE");
                    EnemyStunnedFor = (EnemyStunnedFor - 1); 
                    Debug.Log($"Enemy Skip Counter {EnemyStunnedFor} :: AFTER"); 
                }

                TimeRemaining = SelectionTimer;
                //Debug.Log($"Passed timer reset :: {TimeRemaining}");

                HeroSelectedAction = false;
                //Debug.Log($"Passed SelectedAction Reset :: {HeroSelectedAction}");

                SelectedCard = null;
                //Debug.Log($"Passed Selected Card Reset");

                EnemyAchievedCritical = false;
                //Debug.Log($"Passed EnemyCritical Reset :: {EnemyAchievedCritical}");
                EnemyAchievedCriticalHeal = false;
                //Debug.Log($"Passed HealCritical Reset :: {EnemyAchievedCriticalHeal}");
                HeroAchievedCritical = false;
                //Debug.Log($"Passed HeroCritical Reset :: {HeroAchievedCritical}");

                HandToggles.SetAllTogglesOff(true);
                //Debug.Log($"Passed HandToggles Off");
                HandToggleGroup.transform.GetChild(0).GetComponent<Toggle>().isOn = true;
                //Debug.Log($"Passed set toggle default to on.");
                SelectedCardNameText.SetText("");
                //Debug.Log($"Passed set card name text to null");
            }
        #endregion
    #endregion

    #region GameObjects
        #region Menu Items
            [Header("Game Menu Items")]
            public GameObject MainMenu;
            public GameObject AboutMenu;
            public GameObject GameField;
            public GameObject PauseMenu;
            public GameObject EndingPanel;
            public GameObject BeginPanel;
            public GameObject ConfirmationMenu;
        #endregion
        [Space]
        #region Indicators
            [Header("Game Indicators")]
            public GameObject EnemyStunIndicator;
            public GameObject HeroSkippedIndicator;
            public GameObject EnemyCriticalIndicator;
            public GameObject HeroCritIndicator;
        #endregion
        [Space]
        #region Text Fields
            [Header("Game Text Fields")]
            public TextMeshProUGUI ActionTimer;
            public TextMeshProUGUI HeroHealthText;
            public TextMeshProUGUI EnemyHealthText;
            public TextMeshProUGUI SelectedCardNameText;
        #endregion
        [Space]
        #region Game Buttons
            [Header("Game Buttons")]
            public GameObject Confirm;
            public GameObject Forfeit;
            public GameObject NextRound;
        #endregion
        [Space]
        #region Additional Objects
            [Header("Additional Game Objects")]
            public GameObject Canvas;
            public GameObject HistoryField;
            public GameObject HistoryItem;
            public GameObject RoundEntry;
            public GameObject RoundSpacer;
            public GameObject HandToggleGroup;
            [HideInInspector] public ToggleGroup HandToggles;
    #endregion
    #endregion

    #region GameDetails
        #region Round Details
            [HideInInspector] public bool HeroTurnSkipped = false;
            [HideInInspector] public bool HeroSelectedAction = false;

            [HideInInspector] public bool WaitingForNewRound = false;
            [HideInInspector] public bool ReadyForAction = false;
        #endregion

        #region Game Details
            [HideInInspector] public bool GamePaused = false;
            [HideInInspector] public bool GameOver = false;
            [HideInInspector] public bool HeroDefeated = false;
            [HideInInspector] public bool HeroForfeit = false;
            [HideInInspector] public bool EnemyDefeated = false;
            [HideInInspector] public bool GameIsDraw = false;

            [HideInInspector] public String SelectedCard = null;

            [HideInInspector] public bool HeroExhausted = false;
            [HideInInspector] public bool ExhaustionDetriment = false;
        #endregion

        #region Round Tools
            #region Round Timer
                [Header("Default Value")]
                public float SelectionTimer;
                [HideInInspector] public float TimeRemaining;
                public bool UpdateActionTimer() { 
                    if(TimeRemaining > 0.00f) {
                        TimeRemaining -= Time.deltaTime;

                        if(TimeRemaining == SelectionTimer) 
                            { ActionTimer.color = Color.white; }
                        else if(TimeRemaining > ((SelectionTimer / 3) * 2)) 
                            { ActionTimer.color = Color.yellow; }
                        else if(TimeRemaining <= ((SelectionTimer / 3) * 2) && TimeRemaining > (SelectionTimer / 3)) 
                            { ActionTimer.color = new(255, 165, 0); }
                        else if(TimeRemaining <= (SelectionTimer / 3)) 
                            { ActionTimer.color = Color.red; }

                        TimeSpan ts = System.TimeSpan.FromSeconds(TimeRemaining);
                        ActionTimer.SetText($"{ts:mm\\:ss\\:ff}");

                        if(TimeRemaining <= 0.00f) { ActionTimer.SetText("00:00:00"); } 
                        return true;
                    } else {
                        if(TimeRemaining <= 0.00f) { ActionTimer.SetText("00:00:00"); } 
                        return false;
                    }
                }
            #endregion

            #region Game Pause
                public void Pause() { 
                    if(!GamePaused) { 
                        GamePaused = true;
                        Time.timeScale = 0;
                        PauseMenu.SetActive(true);
                    }
                }
                public void Unpause() { 
                    if(GamePaused) { 
                        GamePaused = false;
                        Time.timeScale = 1;
                        PauseMenu.SetActive(false);
                    }
                }
                public void CheckForPause() {
                    if(GameField.activeInHierarchy && !BeginPanel.activeInHierarchy && !ConfirmationMenu.activeInHierarchy) {
                        if (Input.GetKeyDown(KeyCode.Escape)) {
                            Pause();
                        } 
                    }
                }
                public void CheckForUnpause() { 
                    if(PauseMenu.activeInHierarchy && GamePaused && !ConfirmationMenu.activeInHierarchy) { 
                        if(Input.GetKeyDown(KeyCode.Escape)) { 
                            Unpause();
                        }
                    }
                }
            #endregion

            #region Random Generator
                private Random RandomGenerator = new();
                [HideInInspector] public Random RNG { get { RandomGenerator = new(); return RandomGenerator; } }
            #endregion

            #region History Log
                [HideInInspector] public Stack<GameObject> HistoryLog = new();
                public void CreateHistory(String HistoryContents, int RoundNumber = 0) {
                    //TODO: If RoundNumber > 0 add a space in the history.
                    GameObject HistoryEntry = Instantiate(HistoryItem);
                    TextMeshProUGUI Contents = HistoryEntry.transform.Find("HistoryText").GetComponent<TextMeshProUGUI>();

                    Contents.richText = true;
                    Contents.SetText(HistoryContents);
                    //Create round indicator and setup here.

                    HistoryEntry.transform.SetParent(HistoryField.transform);
                    HistoryEntry.transform.SetAsFirstSibling();
                    HistoryEntry.transform.localScale = Vector3.one;

                    HistoryLog.Push(HistoryEntry);
                }
                public void CreateSpacer() { 
                    GameObject Spacer = Instantiate(RoundSpacer);
                    Spacer.transform.SetParent(HistoryField.transform);
                    Spacer.transform.SetAsFirstSibling();
                    Spacer.transform.localScale = Vector3.one;

                    HistoryLog.Push(Spacer);
                }
                public void ResetHistory() { 
                    foreach(var entry in HistoryLog) {
                        DestroyImmediate(entry);        
                    }
                    HistoryLog.Clear();
                }
                public void FinalizeHistory() {
                    ++Round;
                    GameObject RoundLog = Instantiate(RoundEntry);
                    TextMeshProUGUI Contents = RoundLog.transform.Find("RoundText").GetComponent<TextMeshProUGUI>();

                    Contents.richText = true;
                    Contents.SetText($"Round {Round}");

                    RoundLog.transform.SetParent(HistoryField.transform);
                    RoundLog.transform.SetAsFirstSibling();
                    RoundLog.transform.localScale = Vector3.one;

                    HistoryLog.Push(RoundLog);
                }
            #endregion

            #region Text Tools
                public String GreenText<T>(T param) {
                    if(typeof(T) == typeof(int)) { 
                        return $"<color=green>{param.ToString()}</color>";
                    } else if(typeof(T) == typeof(String) || typeof(T) == typeof(string)) { 
                        return $"<color=green>{param}</color>";
                    } else return null;
                }

                public String RedText<T>(T param) {
                    if(typeof(T) == typeof(int)) { 
                        return $"<color=red>{param.ToString()}</color>";
                    } else if(typeof(T) == typeof(String) || typeof(T) == typeof(string)) { 
                        return $"<color=red>{param}</color>";
                    } else return null;
                }
                public String PurpleText<T>(T param) {
                    if(typeof(T) == typeof(int)) { 
                        return $"<color=purple>{param.ToString()}</color>";
                    } else if(typeof(T) == typeof(String) || typeof(T) == typeof(string)) { 
                        return $"<color=purple>{param}</color>";
                    } else return null;
                }
                public String SilverText<T>(T param) {
                    if(typeof(T) == typeof(int)) { 
                        return $"<color=#C0C0C0>{param.ToString()}</color>";
                    } else if(typeof(T) == typeof(String) || typeof(T) == typeof(string)) { 
                        return $"<color=#C0C0C0>{param}</color>";
                    } else return null;
                }
                public String GoldenText<T>(T param) {
                    if(typeof(T) == typeof(int)) { 
                        return $"<color=#FFD700>{param.ToString()}</color>";
                    } else if(typeof(T) == typeof(String) || typeof(T) == typeof(string)) { 
                        return $"<color=#FFD700>{param}</color>";
                    } else return null;
                }
                public String BlueText<T>(T param) {
                    if(typeof(T) == typeof(int)) { 
                        return $"<color=blue>{param.ToString()}</color>";
                    } else if(typeof(T) == typeof(String) || typeof(T) == typeof(string)) { 
                        return $"<color=blue>{param}</color>";
                    } else return null;
                }
    #endregion
        #endregion
    #endregion

    #region Enemy
        #region Enemy Health
            [Header("Enemy's Health")]
            public int EnemyMaxHealth;
            [HideInInspector] public int EnemyCurrentHealth;
            public void ColorEnemyHealth() { 
                if (EnemyCurrentHealth > ((EnemyMaxHealth / 3) * 2)) {
                    EnemyHealthText.color = Color.green;
                } else if (EnemyCurrentHealth <= (EnemyMaxHealth / 3)) {
                    EnemyHealthText.color = Color.red;
                } else EnemyHealthText.color = Color.yellow;

                if(EnemyCurrentHealth <= 0) { EnemyDefeated = true;}
                //if(EnemyCurrentHealth > EnemyMaxHealth) { EnemyCurrentHealth = EnemyMaxHealth; }

                EnemyHealthText.SetText("Enemy Health: " + EnemyCurrentHealth + " / " + EnemyMaxHealth);
            }
        #endregion
        [Space]
        #region Additional Enemy Details
            [Header("Additional Enemy Details")]
            public int MaximumEnemyStunTurns;
            [HideInInspector] public int EnemyActionAmount;
            private int EnemyStunnedFor = 0;
            [HideInInspector] public bool EnemyTurnSkipped = false;
            //[HideInInspector] public int TurnsEnemyStunned;
            [HideInInspector] public bool EnemyAchievedCritical = false;
            [HideInInspector] public bool EnemyAchievedCriticalHeal = false;
        #endregion
        [Space]
        #region Enemy Card Values
            [Header("Enemy's Effectiveness")]
            public int EnemyAttackDamage;
            public int EnemyHealEffectiveness;
        #endregion
        [Space]
        #region Enemy Critical Values
            [Header("Enemy's Critical Value")]
            public int EnemyAttackCriticalGrowth;
            public int EnemyHealCriticalGrowth;
        #endregion
        [Space]
        #region Enemy Chance Values
            [Header("Enemy's Chance")]
            [Range(0f, 100f)] public float EnemyAttackCriticalChance;
            [Range(0f, 100f)] public float EnemyHealCriticalChance;
        #endregion
        [Space]
        #region Enemy Diminishing Returns
            [Header("Enemy's Diminishing Return")]
            [Range(0f, 100f)] public float EnemyAttackDiminishingReturn;
            [Range(0f, 100f)] public float EnemyHealDiminishingReturn;
        #endregion

        #region Enemy Methods
        public void ResetEnemyHealth() {
            EnemyCurrentHealth = EnemyMaxHealth;
        }
        #endregion
    #endregion

    #region Hero
        #region Hero Health
            [Header("Hero's Health")]
            public int HeroMaxHealth;
            [HideInInspector] public int HeroCurrentHealth;
            public void ColorHeroHealth() { 
                if (HeroCurrentHealth > ((HeroMaxHealth / 3) * 2)) {
                    HeroHealthText.color = Color.green;
                } else if (HeroCurrentHealth < (HeroMaxHealth / 3)) {
                    HeroHealthText.color = Color.red;
                } else HeroHealthText.color = Color.yellow;
                
                if(HeroCurrentHealth <= 0) { HeroDefeated = true; }
                if(HeroCurrentHealth > HeroMaxHealth) { HeroCurrentHealth = HeroMaxHealth; }

                HeroHealthText.SetText("Hero Health: " + HeroCurrentHealth + " / " + HeroMaxHealth);
            }
        #endregion
        [Space]
        #region Additional Hero Details
            [Header("Additional Hero Details")]
            [HideInInspector] public int HeroActionAmount;
            [HideInInspector] public int HeroExhaustionAmount;
            [HideInInspector] public bool HeroAchievedCritical = false;
        #endregion
        [Space]
        #region Hero Card Values
            [Header("Hero's Effectiveness")] 
            public int HeroSlashDamage;
            public int HeroSmashDamage;
            public int HeroDefenseEffectiveness;
            public int HeroHealEffectiveness;
            public int HeroExhaustionDamage;
        #endregion
        [Space]
        #region Hero Critical Values
            [Header("Hero's Critical Value")]
            public int HeroSlashCriticalGrowth;
            public int HeroSmashCriticalGrowth;
            public int HeroDefenseCriticalGrowth;
            public int HeroHealCriticalGrowth;
            public int HeroExhaustionCriticalGrowth;
        #endregion
        [Space]
        #region Hero Chance Values
            [Header("Hero's Chance")]
            [Range(0f, 100f)] public float HeroSlashCriticalChance;
            [Range(0f, 100f)] public float HeroSmashStunChance;
            [Range(0f, 100f)] public float HeroParryCriticalChance;
            [Range(0f, 100f)] public float HeroDefenseCriticalChance;
            [Range(0f, 100f)] public float HeroDefenseStunChance;
            [Range(0f, 100f)] public float HeroHealCriticalChance;
            [Range(0f, 100f)] public float HeroExhaustionCriticalChance;
        #endregion
        [Space]
        #region Hero Diminishing Returns
            [Header("Hero's Diminishing Return")]
            [Range(0f, 100f)] public float HeroSlashDiminishingReturn;
            [Range(0f, 100f)] public float HeroSmashDiminishingReturn;
            [Range(0f, 100f)] public float HeroDefenseDiminishingReturn;
            [Range(0f, 100f)] public float HeroHealDiminishingReturn;
            [Range(0f, 100f)] public float HeroExhaustionDiminishingReturn;
        #endregion

        #region Hero Methods
            public void ResetHeroHealth() {
                HeroCurrentHealth = HeroMaxHealth;
            }
        #endregion
    #endregion

    public void HandleHealth() {
        ColorHeroHealth();
        ColorEnemyHealth();
    }
}