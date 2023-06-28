//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;
//using System.Linq;

//using Random = System.Random;
//using static GameDetails;
//using static GameObjects;
//using static Graphics;
//using static Hero;
//using static Enemy;

//public class Deck : MonoBehaviour {
//    #region Available Cards
//    [HideInInspector] public static Dictionary<string, Card> AvailableCards = new Dictionary<string, Card>() {
//            { "Attack - Slash", new Card("Attack - Slash", SlashSprite) },
//            { "Attack - Smash", new Card("Attack - Smash", SmashSprite) },
//            { "Special - Parry", new Card("Special - Parry", ParrySprite) },
//            { "Defense - Shield", new Card("Defense - Shield", DefenseSprite) },
//            { "Special - Heal", new Card("Special - Heal", HealingSprite) },
//            { "Detriment - Exhaustion", new Card("Detriment - Exhaustion", ExhaustionSprite) }
//        };
//    #endregion

//    #region Hero's Deck
//        [HideInInspector] public static Stack<Card> Library = new Stack<Card>();
//        #region Related Methods
//            public static void InitializeLibrary(GameManager mgr = null) {
//                if(Library != null) Library.Clear();
//                if(Hand != null) ClearHand();
//                if(Graveyard != null) Graveyard.Clear();
//                if(HistoryLog != null) { 
//                    if(mgr != null) mgr.ResetHistory();
//                }

//                ResetEnemyHealth();
//                ResetHeroHealth();

//                List<Card> temp = new List<Card>();
//                temp.AddRange(Enumerable.Repeat(AvailableCards.GetValueOrDefault("Attack - Slash"), 7));
//                temp.AddRange(Enumerable.Repeat(AvailableCards.GetValueOrDefault("Attack - Smash"), 5));
//                temp.AddRange(Enumerable.Repeat(AvailableCards.GetValueOrDefault("Special - Parry"), 2));
//                temp.AddRange(Enumerable.Repeat(AvailableCards.GetValueOrDefault("Defence - Shield"), 5));
//                temp.AddRange(Enumerable.Repeat(AvailableCards.GetValueOrDefault("Special - Heal"), 4));
//                temp.AddRange(Enumerable.Repeat(AvailableCards.GetValueOrDefault("Detriment - Exhaustion"), 7));

//                foreach(Card item in temp) {
//                    Library.Push(item);
//                }
//            }
//        #endregion
//    #endregion

//    #region Hero's Graveyard
//        [HideInInspector] public static List<Card> Graveyard = new List<Card>();
//        [HideInInspector] public static int CardsToLose = 0;
//        public static void DiscardHand() {
//            string UsedCard = null;
//            bool HealDeleted = false;

//            if(SelectedCard != null) { UsedCard = SelectedCard.CardName; }

//            if(ExhaustionDetriment) {
//                List<Card> ReturnToDeck = new List<Card>();
            
//                if(Library.Count() >= CardsToLose) { 
//                    for(int i = 0; i < CardsToLose; ++i) {
//                        Card card = Library.Peek();
//                        while(card.CardName == "Detriment - Exhaustion") { 
//                            ReturnToDeck.Add(Library.Pop());
//                            card = Library.Peek();
//                        }
//                        Library.Pop();
//                    }
//                    foreach(Card item in ReturnToDeck) {
//                        Library.Push(item);
//                    }
//                } else { 
//                    for(int i = 0; i < CardsToLose; ++i) {
//                        int rnd = RNG.Next(0, Graveyard.Count());
//                        var card = Graveyard[rnd];
//                        while(card.CardName == "Detriment - Exhaustion") {
//                            rnd = RNG.Next(0, Graveyard.Count());
//                            card = Graveyard[rnd];
//                        }
//                        Graveyard.RemoveAt(rnd);
//                    }
//                }
//            }

//            if (UsedCard == "Special - Heal") {
//                for (int i = 0; i < Hand.Length; ++i) {
//                    if (Hand[i].CardName == "Special - Heal" && !HealDeleted) {
//                        Hand[i] = null;
//                        HealDeleted = true;
//                    } else Graveyard.Add(Hand[i]);
//                }
//            } else { 
//                for(int i = 0; i < Hand.Length; ++i) {
//                    Graveyard.Add(Hand[i]);
//                    Hand[i] = null;
//                }
//            }
//        }
//    #endregion

//    #region Hero's Hand
//        [HideInInspector] public static Card[] Hand = new Card[5];
//        public static void Draw() {
//            int exhaustion = 0;

//            for(int i = 0; i < 5; ++i) {
//                Transform card = HandToggleGroup.transform.GetChild(i + 1);
//                Toggle cardToggle = card.GetComponent<Toggle>();

//                if(Library.Count == 0) {
//                    Graveyard.Clear();
//                }

//                Hand[i] = Library.Pop();
                
//                if (Hand[i].CardName == "Detriment - Exhaustion") { 
//                    ++exhaustion;
//                    cardToggle.interactable = false;
//                } else cardToggle.interactable = true;

//                card.Find("Background").Find("Card Image").GetComponent<Image>().sprite = Hand[i].CardFace;
//                card.GetChild(1).name = Hand[i].CardName;
//            }

//            if(exhaustion >= 3) {
//                HeroTurnSkipped = true;
//                HeroExhausted = true;

//                DisableHand();

//                if(exhaustion == 5) {
//                    ExhaustionDetriment = true;
//                    CardsToLose = RNG.Next(0, 5);
//                } else { 
//                    ExhaustionDetriment = false;
//                    CardsToLose = 0;
//                }

//            } else {
//                HeroTurnSkipped = false;
//                HeroExhausted = false;
//            }
//        }
//        public static void DisableHand() { 
//            for(int i = 0; i < Hand.Count(); ++i) {
//                HandToggleGroup.transform.GetChild(i + 1).GetComponent<Toggle>().interactable = false;
//            }
//        }
//        public static void ClearHand() { 
//            for(int i = 0; i < Hand.Count(); ++i) {
//                Hand[i] = null;
//            }
//        }
//    #endregion

//    #region Extension Methods
//    private static List<T> Shuffle<T>(List<T> list) {
//        Random rng = new Random();
//        int count = list.Count;

//        for (int i = count - 1; i > 0; --i) {
//            int k = rng.Next(i + 1);
//            T value = list[k];
//            list[k] = list[i];
//            list[i] = value;
//        }

//        return list;
//    }
//    #endregion
//}