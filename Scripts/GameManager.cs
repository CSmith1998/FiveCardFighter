using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
    #region Variables
    [SerializeField, Header("Health Values")] protected int PlayerMaxHealth;
    private int PlayerHealth;
    [SerializeField, Space(25)] protected int EnemyMaxHealth;
    private int EnemyHealth;

    [SerializeField, Header("Menu Items")] protected GameObject MainMenu;
    [SerializeField] protected GameObject AboutMenu;
    [SerializeField] protected GameObject GameField;
    [SerializeField] protected GameObject PauseMenu;
    [SerializeField] protected GameObject EndingPanel;
    [SerializeField, Space(25)] protected GameObject BeginPanel;

    [SerializeField, Header("Indicators")] protected GameObject StunIndicator;
    [SerializeField] protected GameObject SkippedIndicator;
    [SerializeField] protected GameObject eCritIndicator;
    [SerializeField, Space(25)] protected GameObject pCritIndicator;

    [SerializeField, Header("Text Fields")] private TextMeshProUGUI actionTimer;
    [SerializeField] private TextMeshProUGUI txtPlayerHealth;
    [SerializeField] private TextMeshProUGUI txtEnemyHealth;
    [SerializeField, Space(25)] private TextMeshProUGUI selectedCardName;

    [SerializeField, Header("Game Objects")] public GameObject Canvas;
    [SerializeField] protected GameObject HistoryItem;
    [SerializeField] protected GameObject History;
    [SerializeField, Space(25)] private GameObject handToggleGroup;

    [SerializeField, Header("Sprites")] public Sprite ExhaustionSprite;
    [SerializeField] public Sprite HealingSprite;
    [SerializeField] public Sprite SlashSprite;
    [SerializeField] public Sprite SmashSprite;
    [SerializeField] public Sprite ParrySprite;
    [SerializeField, Space(25)] public Sprite DefendSprite;

    [SerializeField, Header("Buttons")] protected GameObject Select;
    [SerializeField] protected GameObject Forfeit;
    [SerializeField, Space(25)] protected GameObject NextTurn;

    [SerializeField, Header("Critical Values")] protected int EnemyCriticalBonus;
    [SerializeField] protected float EnemyBasicCriticalChance;
    [SerializeField] protected float EnemyHealingCriticalChance;
    [SerializeField] protected float EnemyBasicDiminishingReturn;
    [SerializeField] protected float EnemyHealingDiminishingReturn;
    [SerializeField] protected int ExhaustionCriticalBonus;
    [SerializeField] protected float ExhaustionCriticalChance;
    [SerializeField] protected float ExhaustionDiminishingReturn;
    [SerializeField] protected int PlayerCriticalBonus;
    [SerializeField] protected float PlayerBasicCriticalChance;
    [SerializeField] protected float PlayerBasicDiminishingReturn;
    [SerializeField] protected float PlayerSmashCriticalChance;
    [SerializeField] protected float PlayerSmashDiminishingReturn;
    [SerializeField] protected float PlayerHealingCriticalChance;
    [SerializeField] protected float PlayerHealingDiminishingReturn;
    [SerializeField] protected float PlayerDefenseStunChance;
    [SerializeField] protected float PlayerShieldStunChance;
    [SerializeField, Space(25)] protected float PlayerSmashStunChance;

    [SerializeField, Header("Default Values")] protected float TimerAmount;
    [SerializeField] public int PlayerSlashDamage;
    [SerializeField] public int PlayerSmashDamage;
    [SerializeField] protected int PlayerParryDamage;
    [SerializeField] public int PlayerDefenseAmount;
    [SerializeField] public int MaxStunTurns;
    [SerializeField] public int PlayerHealAmount;
    [SerializeField] public int ExhaustionDamage;
    [SerializeField] protected int EnemyAttackDamage;
    [SerializeField, Space(25)] protected int EnemyHealAmount;
    

    private Dictionary<string, Card> CardOptions = new Dictionary<string, Card>();
    private Stack<Card> Deck = new Stack<Card>();
    private List<Card> Discard = new List<Card>();
    private Card[] Hand = new Card[5];

    private Stack<GameObject> HistoryLog = new Stack<GameObject>();

    private bool isPaused = false;
    private bool skipTurn = false;
    private bool selectionMade = false;
    private bool playerParrying = false;
    private bool playerDefending = false;
    private bool playerDefenseStun = false;
    private bool ExhaustDetriment = false;

    private bool PlayerTurn = true;
    private bool EnemyTurn = false;
    private bool EndOfRound = false;
    private bool TurnInProgress = false;

    private bool PlayerDefeated = false;
    private bool EnemyDefeated = false;
    private bool GameOver = false;
    private bool GameDraw = false;
    private bool PlayerForfeit = false;

    private bool EnemyCrit = false;
    private bool PlayerCrit = false;

    private bool WaitingForNewRound = false;

    private float timer = 30.00f;

    private Card selected = null;

    private ToggleGroup HandToggles;

    private System.Random rng = new System.Random();

    private int playerActionAmount = 0;
    private int playerExhaustionDamage = 0;
    private int enemyActionAmount = 0;
    private int enemyStunnedTurns = 0;
    #endregion

    private void Start() {
        HandToggles = handToggleGroup.GetComponent<ToggleGroup>();
        InitializeCardOptions();

        PlayerHealth = PlayerMaxHealth;
        EnemyHealth = EnemyMaxHealth;
    }

    private void Update() {
        if(GameField.activeInHierarchy && !GameOver) {
            CheckForPause();
            
            if(!isPaused) {
                if(enemyStunnedTurns > 0) {
                    StunIndicator.SetActive(true);
                } else { StunIndicator.SetActive(false); }

                if(skipTurn) {
                    SkippedIndicator.SetActive(true);
                    Select.SetActive(false);
                } else if(!skipTurn) {
                    SkippedIndicator.SetActive(false);
                    if(!WaitingForNewRound) { Select.SetActive(true); }
                }

                if(WaitingForNewRound) {
                    NextTurn.SetActive(true);
                    Select.SetActive(false);
                } else NextTurn.SetActive(false);

                if(EnemyCrit) {
                    pCritIndicator.SetActive(true);
                } else {
                    pCritIndicator.SetActive(false);
                }

                if(PlayerCrit) {
                    eCritIndicator.SetActive(true);
                } else {
                    eCritIndicator.SetActive(false);
                }

                RoundHandler();
            }
        }
    }

    private void RoundHandler() {
        if(!WaitingForNewRound) {
            if(!skipTurn && !selectionMade) {
                if(timer >= 0.00f) {
                    var active = HandToggles.ActiveToggles().FirstOrDefault();

                    if(active != null) { 
                        if(active.name == "Default") {
                            selectedCardName.SetText("");
                        } else selectedCardName.SetText(active.gameObject.transform.GetChild(1).name);
                    }

                    ActionTimer();
                }

                if(timer <= 0.00f) { skipTurn = true; }
            }

            if(timer <= 0.00f || selectionMade || skipTurn) {
                if (PlayerTurn && !TurnInProgress) { PlayerAction(); }
                if (EnemyTurn && !TurnInProgress) { EnemyAction(); }
                if (EndOfRound && !TurnInProgress) { EndOfTurn(); }
            }
        }
    }

    public void PlayerAction() {
        TurnInProgress = true;
        int EST = enemyStunnedTurns;

        if(!skipTurn) {
            if(selected.CardName == "Special - Parry") {
                playerParrying = true;
            } else playerActionAmount = CalculateCritical("Player", selected.CardName);

            switch(selected.CardName) {
                case "Attack - Slash":
                    if(PlayerCrit) {
                        CreateHistory(string.Format("The <color=green>Hero</color> dealt a <color=red>critical</color> blow, doing <color=green>{0}</color> damage!", playerActionAmount));
                    } else CreateHistory(string.Format("The <color=green>Hero</color> dealt <color=green>{0}</color> damage to the <color=red>enemy</color>.", playerActionAmount));
                    EnemyHealth -= playerActionAmount;
                    break;
                case "Attack - Smash":
                    if(PlayerCrit) {
                        CreateHistory(string.Format("The <color=green>Hero</color> dealt <color=green>{0}</color> damage to the <color=red>enemy</color>, <color=purple>stunning</color> them for <color=purple>{1}</color> turn(s)!", playerActionAmount, (enemyStunnedTurns - EST)));
                    } else CreateHistory(string.Format("The <color=green>Hero</color> dealt <color=green>{0}</color> damage to the <color=red>enemy</color>.", playerActionAmount));
                    EnemyHealth -= playerActionAmount;
                    break;
                case "Special - Heal":
                    PlayerHealth = PlayerHealth + playerActionAmount;
                    if(PlayerHealth > PlayerMaxHealth) { PlayerHealth = PlayerMaxHealth; }
                    
                    if(PlayerCrit) {
                        CreateHistory(string.Format("The <color=green>Hero</color> obtained a <color=red>critical</color>, healing <color=green>{0}</color> damage!", playerActionAmount));
                    } else CreateHistory(string.Format("The <color=green>Hero</color> healed <color=green>{0}</color> damage.", playerActionAmount));
                    break;
            } 
        } else if(ExhaustDetriment) {
            playerExhaustionDamage = CalculateCritical("Player", "Detriment - Exhaustion");
            PlayerHealth -= playerExhaustionDamage;
            if(PlayerCrit) {
                CreateHistory(string.Format("The <color=green>Hero</color> was <color=red>critically</color> <color=purple>exhausted</color>, taking <color=red>{0}</color> damage!", playerExhaustionDamage));
            } else CreateHistory(string.Format("The <color=green>Hero</color> took <color=red>{0}</color> <color=purple>exhaustion</color> damage.", playerExhaustionDamage));
        } else if(skipTurn) {
            CreateHistory("<color=purple>The Hero's turn was skipped!</color>");
        }

        HandleHealth();
        PlayerTurn = false;
        EnemyTurn = true;
        TurnInProgress = false;
    }

    public void CreateHistory(string action) {
        GameObject temp = Instantiate(HistoryItem);
        var txt = temp.transform.Find("HistoryText").GetComponent<TextMeshProUGUI>();
        txt.richText = true;
        txt.SetText(action);
        temp.transform.SetParent(History.transform);
        temp.transform.SetAsFirstSibling();
        temp.transform.localScale = Vector3.one;
        //HistoryLog.Push(temp);
    }

    public void EnemyAction() {
        TurnInProgress = true;

        if(skipTurn) {
            enemyActionAmount = CalculateCritical("Enemy", "Heal");
            EnemyHealth = EnemyHealth + enemyActionAmount;
            if(EnemyHealth > EnemyMaxHealth) { 
                EnemyHealth = EnemyMaxHealth; 
            }

            if(EnemyCrit) {
                CreateHistory(string.Format("<color=red>Enemy</color> obtained a <color=red>critical</color>, healing <color=red>{0}</color> damage!", enemyActionAmount));
            } else CreateHistory(string.Format("<color=red>Enemy</color> healed <color=red>{0}</color> damage.", enemyActionAmount));
        }

        if(enemyStunnedTurns == 0 && EnemyHealth > 0) {
            enemyActionAmount = CalculateCritical("Enemy", "Attack");

            if (playerDefending) { 
                if(playerActionAmount > enemyActionAmount && playerDefenseStun) {
                    CreateHistory(string.Format("The <color=green>Hero</color> <color=#c0c0c0>shielded</color> <color=green>{0}</color> of <color=red>{1}</color> damage, <color=purple>stunning</color> the <color=red>enemy</color>!", playerActionAmount, enemyActionAmount));
                    ++enemyStunnedTurns;
                } else if(playerActionAmount < enemyActionAmount) {
                    CreateHistory(string.Format("The <color=green>Hero</color> <color=#c0c0c0>shielded</color> <color=green>{0}</color> of <color=red>{1}</color> damage, taking <color=red>{2}</color> damage!", playerActionAmount, enemyActionAmount, (enemyActionAmount - playerActionAmount)));
                    PlayerHealth -= (enemyActionAmount - playerActionAmount);
                } else {
                    CreateHistory(string.Format("The <color=green>Hero</color> <color=#c0c0c0ff>shielded</color> themselves from <color=red>{0}</color> damage.", enemyActionAmount));
                }
            } else if(playerParrying) {
                CreateHistory(string.Format("The <color=green>Hero</color> parried, returning <color=green>{0}</color> damage from the <color=red>enemy</color>!", enemyActionAmount));
                EnemyHealth -= enemyActionAmount;
            } else {
                if(EnemyCrit) {
                    CreateHistory(string.Format("<color=red>Enemy</color> dealt a <color=red>critical</color> blow, doing <color=red>{0}</color> damage!", enemyActionAmount));
                } else CreateHistory(string.Format("<color=red>Enemy</color> inflicted <color=red>{0}</color> damage to the <color=green>Hero</color>!", enemyActionAmount));
                PlayerHealth -= enemyActionAmount;
            } 
        }

        HandleHealth();
        EnemyTurn = false;
        EndOfRound = true;
        TurnInProgress = false;
    }

    public void HandleHealth() {
        txtPlayerHealth.SetText("Player Health: " + PlayerHealth + " / " + PlayerMaxHealth);
        txtEnemyHealth.SetText("Enemy Health: " + EnemyHealth + " / " + EnemyMaxHealth);

        if (PlayerHealth <= (PlayerMaxHealth / 2)) {
            txtPlayerHealth.color = Color.red;
        } else if (PlayerHealth == PlayerMaxHealth) {
            txtPlayerHealth.color = Color.green;
        } else txtPlayerHealth.color = Color.yellow;

        if (EnemyHealth <= (EnemyMaxHealth / 2)) {
            txtEnemyHealth.color = Color.red;
        } else if (EnemyHealth == EnemyMaxHealth) {
            txtEnemyHealth.color = Color.green;
        } else txtEnemyHealth.color = Color.yellow;
    }

    public void EndOfTurn() {
        TurnInProgress = true;
        WaitingForNewRound = true;

        DiscardHand();

        if(EnemyHealth <= 0) { EnemyDefeated = true; }
        if(PlayerHealth <= 0) { PlayerDefeated = true; }
        if(PlayerDefeated || EnemyDefeated || PlayerForfeit) {
            GameOver = true;
            if(PlayerDefeated && EnemyDefeated) { GameDraw = true; }
            EndOfGame();
        }
        EndOfRound = false;
        TurnInProgress = false;
    }

    public void EndOfGame() { 
        if(GameOver) {
            var title = EndingPanel.transform.Find("EndTitle").GetComponent<TextMeshProUGUI>();
            var text = EndingPanel.transform.Find("EndDetails").GetComponent<TextMeshProUGUI>();

            if(PlayerDefeated && !GameDraw) {
                title.SetText("DEFEATED");
                text.SetText("The Hero was defeated by their enemy! Better luck next time...");
                title.color = Color.red;
            }
            if(EnemyDefeated && !GameDraw) {
                title.SetText("VICTORY");
                text.SetText("The Hero was victorious over their enemy! Onwards, to fame and fortune!");
                title.color = Color.green;
            }
            if(GameDraw) {
                title.SetText("DRAW");
                text.SetText("The Hero and their enemy lie exhausted, unable to continue their fight. No one wins on this day!");
                title.color = Color.yellow;
            }
            if(PlayerForfeit) {
                title.SetText("Forfeit");
                text.SetText("The Hero withdraws from the battle, battered and bruised. Their enemy lives to see another day...");
                title.color = Color.red;
            }

            EndingPanel.SetActive(true);
        }
    }

    public void ClickedNextTurn() {
        if(!GameOver) {
            ResetValues();
            Draw();
            PlayerTurn = true;
            WaitingForNewRound = false;
        }
    }

    public void ResetValues() {
        timer = TimerAmount;
        selectionMade = false;
        playerParrying = false;
        playerDefending = false;
        playerDefenseStun = false;
        ExhaustDetriment = false;
        playerActionAmount = 0;
        playerExhaustionDamage = 0;
        enemyActionAmount = 0;

        selected = null;

        EnemyCrit = false;
        PlayerCrit = false;

        skipTurn = false;
        if(enemyStunnedTurns > 0 && EnemyHealth >= 0) { 
            --enemyStunnedTurns; 
        }

        HandToggles.SetAllTogglesOff(true);
        selectedCardName.SetText("");
    }

    public void DiscardHand() {
        bool healDeleted = false;
        string usedCardName = "";
        if(selected != null) { usedCardName = selected.CardName; }

        if(ExhaustDetriment) {

            int cardsToLose = rng.Next(0, 5);
            List<Card> returnToDeck = new List<Card>();
            
            if(Deck.Count() >= cardsToLose) { 
                for(int i = 0; i < cardsToLose; ++i) {
                    var card = Deck.Peek();
                    while(card.CardName == "Detriment - Exhaustion") { 
                        returnToDeck.Add(Deck.Pop());
                        card = Deck.Peek();
                    }
                    Deck.Pop();
                }
                foreach(Card item in returnToDeck) {
                    Deck.Push(item);
                }
            } else { 
                for(int i = 0; i < cardsToLose; ++i) {
                    int rnd = rng.Next(0, Discard.Count());
                    var card = Discard[rnd];
                    while(card.CardName == "Detriment - Exhaustion") {
                        rnd = rng.Next(0, Discard.Count());
                        card = Discard[rnd];
                    }
                    Discard.RemoveAt(rnd);
                }
            }
        }

        if (usedCardName == "Special - Heal") {
            for (int i = 0; i < Hand.Length; ++i) {
                if (Hand[i].CardName == "Special - Heal" && !healDeleted) {
                    Hand[i] = null;
                    healDeleted = true;
                } else Discard.Add(Hand[i]);
            }
        } else { 
            for(int i = 0; i < Hand.Length; ++i) {
                Discard.Add(Hand[i]);
                Hand[i] = null;
            }
        }
    }

    public int CalculateCritical(string origin, string action) {
        bool isCritical = true;
        bool setupFlag = true;

        float criticalChance = 0.0f;
        int barToBeat = 100;
        int defenseStun = (int)Math.Round(100 - (100 * PlayerDefenseStunChance));

        int stunTurns = 0;
        int result = 0;

        if(origin != "Enemy") {
            criticalChance = PlayerBasicCriticalChance;
            while(isCritical) {
                switch (action) {
                    case "Attack - Slash":
                        if(setupFlag) { result = PlayerSlashDamage; }
                        barToBeat = (int)Math.Round(100 - (100 * criticalChance));
                        if(rng.Next(101) >= barToBeat) {
                            PlayerCrit = true;
                            criticalChance -= (criticalChance * PlayerBasicDiminishingReturn);
                            result += PlayerCriticalBonus;
                        } else isCritical = false;
                        break;
                    case "Attack - Smash":
                        if(setupFlag) { result = PlayerSmashDamage; criticalChance = PlayerSmashCriticalChance; }
                        barToBeat = (int)Math.Round(100 - (100 * criticalChance));
                        if(rng.Next(101) >= barToBeat && stunTurns < MaxStunTurns) {
                            PlayerCrit = true;
                            criticalChance -= (criticalChance * PlayerSmashDiminishingReturn);
                            ++stunTurns;
                            Debug.Log("Stun Turns: " + stunTurns);
                        } else isCritical = false;
                        break;
                    case "Defence - Shield":
                        if(setupFlag) { 
                            result = PlayerDefenseAmount; 
                            playerDefending = true;
                            if (rng.Next(101) >= defenseStun) { playerDefenseStun = true; }
                        }
                        if(rng.Next(101) >= barToBeat) {
                            PlayerCrit = true;
                            criticalChance -= (criticalChance * PlayerBasicDiminishingReturn);
                            result += PlayerCriticalBonus;
                        } else isCritical = false;
                        break;
                    case "Special - Heal":
                        if(setupFlag) { result = PlayerHealAmount; criticalChance = PlayerHealingCriticalChance; }
                        barToBeat = (int)Math.Round(100 - (100 * criticalChance));
                        if (rng.Next(101) >= barToBeat) {
                            PlayerCrit = true;
                            criticalChance -= (criticalChance * PlayerHealingDiminishingReturn);
                            result += PlayerCriticalBonus;
                        } else isCritical = false;
                        break;
                    case "Detriment - Exhaustion":
                        if(setupFlag) { result = ExhaustionDamage; criticalChance = ExhaustionCriticalChance; }
                        barToBeat = (int)Math.Round(100 - (100 * criticalChance));
                        if (rng.Next(101) >= barToBeat) {
                            PlayerCrit = true;
                            criticalChance -= (criticalChance * ExhaustionDiminishingReturn);
                            result += ExhaustionCriticalBonus;
                        } else isCritical = false;
                        break;
                }
                setupFlag = false;
            }
        } else if(origin == "Enemy") {
            criticalChance = EnemyBasicCriticalChance;
            while (isCritical) { 
                switch(action) { 
                    case "Attack":
                        if(setupFlag) { result = EnemyAttackDamage; }
                        barToBeat = (int)Math.Round(100 - (100 * criticalChance));
                        if(rng.Next(101) >= barToBeat) {
                            EnemyCrit = true;
                            criticalChance -= (criticalChance * EnemyBasicDiminishingReturn);
                            result += EnemyCriticalBonus;
                        } else isCritical = false;
                        break;
                    case "Heal":
                        if(setupFlag) { result = EnemyHealAmount; criticalChance = EnemyHealingCriticalChance; }
                        barToBeat = (int)Math.Round(100 - (100 * criticalChance));
                        if (rng.Next(101) >= barToBeat) {
                            EnemyCrit = true;
                            criticalChance -= (criticalChance * EnemyHealingDiminishingReturn);
                            result += EnemyCriticalBonus;
                        } else isCritical = false;
                        break;
                }
                setupFlag = false;
            }
        }

        if(stunTurns > 0) { enemyStunnedTurns += stunTurns; }
        return result;
    }

    public void ClickedStartGame() {
        if(MainMenu.activeInHierarchy) {
            MainMenu.SetActive(false);
            isPaused = true;
        }
        GameField.SetActive(true);
        BeginPanel.SetActive(true);

        InitializeDeck();
    }

    public void ClickedHelp() {
        AboutMenu.SetActive(true);
    }
    public void ClickedExit() {
        AboutMenu.SetActive(false);
    }
    public void ClickedQuit() {
        Application.Quit();
    }

    public void ClickedForfeit() {
        GameOver = true;
        PlayerForfeit = true;
        EndOfGame();
    }

    public void ReturnToMenu() { 
        if(GameField.activeInHierarchy) {
            GameField.SetActive(false);
        }
        if(EndingPanel.activeInHierarchy) {
            EndingPanel.SetActive(false);
        }
        ResetValues();
        GameOver = false;
        PlayerForfeit = false;
        EnemyDefeated = false;
        PlayerDefeated = false;
        MainMenu.SetActive(true);
    }

    public void ClickedBegin() {
        BeginPanel.SetActive(false);
        timer = 30.00f;
        isPaused = false;

        handToggleGroup.transform.GetChild(0).GetComponent<Toggle>().isOn = true;

        Draw();
    }

    public void ClickedSubmit() {
        if(HandToggles.ActiveToggles().First().name != "Default") {
            var selection = HandToggles.ActiveToggles().First().gameObject;
            selected = CardOptions.GetValueOrDefault(selection.transform.GetChild(1).name);
            selectionMade = true;
        }
    }

    private void CheckForPause() {
        if(GameField.activeInHierarchy && !BeginPanel.activeInHierarchy) {
            if (Input.GetKey(KeyCode.Escape)) {
                isPaused = true;
                Time.timeScale = 0;
                PauseMenu.SetActive(true);
            } 
        }
    }

    public void UnpauseGame() {
        isPaused = false;

        if(PauseMenu.activeInHierarchy) {
            Time.timeScale = 1;
            PauseMenu.SetActive(false);
        }
    }

    private void ActionTimer() { 
        if(timer >= 0.00f) {
            timer -= Time.deltaTime;

            var ts = System.TimeSpan.FromSeconds(timer);
            actionTimer.SetText($"{ts:mm\\:ss\\:ff}");

            if(timer <= 10.00f) {
                actionTimer.color = Color.red;
            } else actionTimer.color = Color.white;
        }
    }

    public void InitializeCardOptions() {
        if(CardOptions == null || CardOptions.Count == 0) {
            CardOptions.Add("Detriment - Exhaustion", new Card("Detriment - Exhaustion", ExhaustionSprite));
            CardOptions.Add("Special - Heal", new Card("Special - Heal", HealingSprite));
            CardOptions.Add("Attack - Slash", new Card("Attack - Slash", SlashSprite));
            CardOptions.Add("Attack - Smash", new Card("Attack - Smash", SmashSprite));
            CardOptions.Add("Special - Parry", new Card("Special - Parry", ParrySprite));
            CardOptions.Add("Defence - Shield", new Card("Defence - Shield", DefendSprite));
        }
    }

    public void InitializeDeck() {
        if (Deck.Count() > 0) { Deck.Clear(); }
        if (Discard.Count() > 0) { Discard.Clear(); }

        List<Card> temp = new List<Card>();
        temp.AddRange(Enumerable.Repeat(CardOptions.GetValueOrDefault("Detriment - Exhaustion"), 7));
        temp.AddRange(Enumerable.Repeat(CardOptions.GetValueOrDefault("Special - Heal"), 4));
        temp.AddRange(Enumerable.Repeat(CardOptions.GetValueOrDefault("Special - Parry"), 2));
        temp.AddRange(Enumerable.Repeat(CardOptions.GetValueOrDefault("Defence - Shield"), 5));
        temp.AddRange(Enumerable.Repeat(CardOptions.GetValueOrDefault("Attack - Slash"), 7));
        temp.AddRange(Enumerable.Repeat(CardOptions.GetValueOrDefault("Attack - Smash"), 5));

        EstablishDeck(Shuffle(temp));
    }

    public void Draw() {
        int exhaustion = 0;

        for(int i = 0; i < 5; ++i) { 
            if(Deck.Count == 0) {
                EstablishDeck(Shuffle(Discard));
                Discard.Clear();
            }

            Hand[i] = Deck.Pop();
            if (Hand[i].CardName == "Detriment - Exhaustion") { 
                ++exhaustion;
                handToggleGroup.transform.GetChild(i + 1).GetComponent<Toggle>().interactable = false;
            } else handToggleGroup.transform.GetChild(i + 1).GetComponent<Toggle>().interactable = true;

            var card = handToggleGroup.transform.GetChild(i + 1);
            card.Find("Background").Find("Card Image").GetComponent<Image>().sprite = Hand[i].CardFace;
            card.GetChild(1).name = Hand[i].CardName;
        }

        if(exhaustion >= 3) {
            skipTurn = true;
            for(int i = 0; i < Hand.Count(); ++i) {
                handToggleGroup.transform.GetChild(i + 1).GetComponent<Toggle>().interactable = false;
            }
            if(exhaustion == 5) {
                ExhaustDetriment = true;
            }
        }
    }

    private List<Card> Shuffle(List<Card> deck) {
        for(int i = deck.Count() - 1; i > 0; --i) {
            int k = rng.Next(i + 1);
            Card value = deck[k];
            deck[k] = deck[i];
            deck[i] = value;
        }
        return deck;
    }

    private void EstablishDeck(List<Card> cards) { 
        foreach(Card item in cards) {
            Deck.Push(item);
        }
    }
}
