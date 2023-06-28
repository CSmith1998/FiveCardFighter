//using System.Collections.Generic;
//using UnityEngine;

//using Random = System.Random;

//using static GameObjects;
//using System;
//using TMPro;

//public class GameDetails : MonoBehaviour {
//    #region Round Details
//    [HideInInspector] public static bool HeroTurnSkipped = false;
//        [HideInInspector] public static bool HeroSelectedAction = false;

//        [HideInInspector] public static bool WaitingForNewRound = false;
//        [HideInInspector] public static bool ReadyForAction = false;
//    #endregion

//    #region Game Details
//        [HideInInspector] public static bool GamePaused = false;
//        [HideInInspector] public static bool GameOver = false;
//        [HideInInspector] public static bool HeroDefeated = false;
//        [HideInInspector] public static bool HeroForfeit = false;
//        [HideInInspector] public static bool EnemyDefeated = false;
//        [HideInInspector] public static bool GameIsDraw = false;

//        [HideInInspector] public static Card SelectedCard = null;

//        [HideInInspector] public static bool HeroExhausted = false;
//        [HideInInspector] public static bool ExhaustionDetriment = false;
//    #endregion

//    #region Round Tools
//        #region Round Timer
//            [Header("Default Value")]
//            public static float SelectionTimer;
//            [HideInInspector] public static float TimeRemaining {
//                get { return TimeRemaining; }
//                set { 
//                    TimeRemaining = value;
//                    if(TimeRemaining == SelectionTimer) 
//                        { ActionTimer.color = Color.white; }
//                    else if(TimeRemaining > ((SelectionTimer / 3) * 2)) 
//                        { ActionTimer.color = Color.yellow; }
//                    else if(TimeRemaining <= ((SelectionTimer / 3) * 2) && TimeRemaining > (SelectionTimer / 3)) 
//                        { ActionTimer.color = new(255, 165, 0); }
//                    else if(TimeRemaining <= (SelectionTimer / 3)) 
//                        { ActionTimer.color = Color.red; }

//                    if(TimeRemaining <= 0.00f) { ActionTimer.SetText("00:00:00"); } 
//                }
//            }
//            public static bool UpdateActionTimer() { 
//                if(TimeRemaining > 0.00f) {
//                    TimeRemaining -= Time.deltaTime;

//                    TimeSpan ts = System.TimeSpan.FromSeconds(TimeRemaining);
//                    ActionTimer.SetText($"{ts:mm\\:ss\\:ff}");
//                    return true;
//                } else return false;
//            }
//        #endregion

//        #region Game Pause
//            public static void Pause() { 
//                GamePaused = true;
//                Time.timeScale = 0;
//                PauseMenu.SetActive(true);
//            }
//            public static void Unpause() { 
//                GamePaused = false;
//                Time.timeScale = 1;
//                PauseMenu.SetActive(false);
//            }
//            public static void CheckForPause() {
//                if(GameField.activeInHierarchy && !BeginPanel.activeInHierarchy) {
//                    if (Input.GetKey(KeyCode.Escape)) {
//                        Pause();
//                    } 
//                }
//            }
//            public static void CheckForUnpause() { 
//                if(PauseMenu.activeInHierarchy && GamePaused) { 
//                    if(Input.GetKey(KeyCode.Escape)) { 
//                        Unpause();
//                    }
//                }
//            }
//        #endregion

//        #region Random Generator
//            private static Random RandomGenerator = new();
//            [HideInInspector] public static Random RNG { get { RandomGenerator = new(); return RandomGenerator; } }
//        #endregion

//        #region History Log
//            [HideInInspector] public static Stack<GameObject> HistoryLog = new();
//            public static void CreateHistory(String HistoryContents, int RoundNumber = 0) {
//                //TODO: If RoundNumber > 0 add a space in the history.
//                GameObject HistoryEntry = UnityEngine.Object.Instantiate(HistoryItem);
//                TextMeshProUGUI Contents = HistoryEntry.transform.Find("HistoryText").GetComponent<TextMeshProUGUI>();

//                Contents.richText = true;
//                Contents.SetText(HistoryContents);
//                //Create round indicator and setup here.

//                HistoryEntry.transform.SetParent(HistoryField.transform);
//                HistoryEntry.transform.SetAsFirstSibling();
//                HistoryEntry.transform.localScale = Vector3.one;

//                HistoryLog.Push(HistoryEntry);
//            }
//            public static void FinalizeHistory() {
//                //TODO: Update round number
//            }
//        #endregion

//        #region Text Tools
//            public static String GreenText<T>(T param) {
//                if(typeof(T) == typeof(int)) { 
//                    return $"<color=green>{param.ToString()}</color>";
//                } else if(typeof(T) == typeof(String) || typeof(T) == typeof(string)) { 
//                    return $"<color=green>{param}</color>";
//                } else return null;
//            }

//            public static String RedText<T>(T param) {
//                if(typeof(T) == typeof(int)) { 
//                    return $"<color=green>{param.ToString()}</color>";
//                } else if(typeof(T) == typeof(String) || typeof(T) == typeof(string)) { 
//                    return $"<color=green>{param}</color>";
//                } else return null;
//            }
//            public static String PurpleText<T>(T param) {
//                if(typeof(T) == typeof(int)) { 
//                    return $"<color=purple>{param.ToString()}</color>";
//                } else if(typeof(T) == typeof(String) || typeof(T) == typeof(string)) { 
//                    return $"<color=purple>{param}</color>";
//                } else return null;
//            }
//            public static String SilverText<T>(T param) {
//                if(typeof(T) == typeof(int)) { 
//                    return $"<color=#C0C0C0>{param.ToString()}</color>";
//                } else if(typeof(T) == typeof(String) || typeof(T) == typeof(string)) { 
//                    return $"<color=#C0C0C0>{param}</color>";
//                } else return null;
//            }
//            public static String GoldenText<T>(T param) {
//                if(typeof(T) == typeof(int)) { 
//                    return $"<color=#FFD700>{param.ToString()}</color>";
//                } else if(typeof(T) == typeof(String) || typeof(T) == typeof(string)) { 
//                    return $"<color=#FFD700>{param}</color>";
//                } else return null;
//            }
//            public static String BlueText<T>(T param) {
//                if(typeof(T) == typeof(int)) { 
//                    return $"<color=blue>{param.ToString()}</color>";
//                } else if(typeof(T) == typeof(String) || typeof(T) == typeof(string)) { 
//                    return $"<color=blue>{param}</color>";
//                } else return null;
//            }
//        #endregion
//    #endregion

//}