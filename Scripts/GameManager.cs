using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

using static Deck;
using static Enemy;
using static GameDetails;
using static GameMechanics;
using static GameObjects;
using static Graphics;
using static Hero;

public class GameManager : MonoBehaviour {
    #region Variables
    //private bool playerParrying = false;
    //private bool playerDefending = false;
    //private bool playerDefenseStun = false;

    //private bool PlayerTurn = true;
    //private bool EnemyTurn = false;
    //private bool EndOfRound = false;
    //private bool TurnInProgress = false;
    #endregion

    private void Start() {
        HandToggles = HandToggleGroup.GetComponent<ToggleGroup>();
    }

    private void Update() {
        if(GameField.activeInHierarchy && !GameOver) {
            if(!GamePaused) { CheckForPause();
                IndicatorHandler();
                RoundHandler();
            } else CheckForUnpause();
        }
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
        exhausted = false;
        if(enemyStunnedTurns > 0 && EnemyHealth >= 0) { 
            --enemyStunnedTurns; 
        }

        HandToggles.SetAllTogglesOff(true);
        selectedCardName.SetText("");
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
            //ResetValues();
            //GameOver = false;
            //HeroForfeit = false;
            //EnemyDefeated = false;
            //HeroDefeated = false;
            MainMenu.SetActive(true);
        }
        public void ClickedStartGame() {
            if(MainMenu.activeInHierarchy) {
                MainMenu.SetActive(false);
                GamePaused = true;
            }
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
            if(HandToggles.ActiveToggles().First().name != "Default") {
                var selection = HandToggles.ActiveToggles().First().gameObject;
                SelectedCard = AvailableCards.GetValueOrDefault(selection.transform.GetChild(1).name);
                SelectionMade = true;
            }
        }
        public void ClickedNextTurn() {
            if(!GameOver) {
                ResetValues();
                Draw();
                //PlayerTurn = true;
                WaitingForNewRound = false;
            }
        }
        public void ClickedForfeit() {
            GameOver = true;
            HeroForfeit = true;
            EndOfGame();
        }
        public static void ClickedResume() {
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
}