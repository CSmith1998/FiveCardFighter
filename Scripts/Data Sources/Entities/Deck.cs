using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using Random = System.Random;
using static Graphics;

public static class Deck {

    #region Available Cards
    [HideInInspector] public static Dictionary<string, Card> AvailableCards = new Dictionary<string, Card>() {
        { "Slash", new Card("Attack - Slash", SlashSprite) },
        { "Smash", new Card("Attack - Smash", SmashSprite) },
        { "Parry", new Card("Special - Parry", ParrySprite) },
        { "Defend", new Card("Defense - Shield", DefenseSprite) },
        { "Heal", new Card("Special - Heal", HealingSprite) },
        { "Detriment", new Card("Detriment - Exhaustion", ExhaustionSprite) }
    };
    #endregion

    #region Hero's Deck
    [HideInInspector] public static Stack<Card> Library = new Stack<Card>();
    #region Related Methods
    public static void InitializeLibrary() {
        Library.Clear();

        List<Card> temp = new List<Card>();
        temp.AddRange(Enumerable.Repeat(AvailableCards.GetValueOrDefault("Attack - Slash"), 7));
        temp.AddRange(Enumerable.Repeat(AvailableCards.GetValueOrDefault("Attack - Smash"), 5));
        temp.AddRange(Enumerable.Repeat(AvailableCards.GetValueOrDefault("Special - Parry"), 2));
        temp.AddRange(Enumerable.Repeat(AvailableCards.GetValueOrDefault("Defence - Shield"), 5));
        temp.AddRange(Enumerable.Repeat(AvailableCards.GetValueOrDefault("Special - Heal"), 4));
        temp.AddRange(Enumerable.Repeat(AvailableCards.GetValueOrDefault("Detriment - Exhaustion"), 7));

        //EstablishDeck(Shuffle(temp));
    }
    #endregion
    #endregion

    #region Hero's Graveyard
    [HideInInspector] public static List<Card> Graveyard = new List<Card>();
    #endregion

    #region Hero's Hand
    [HideInInspector] public static Card[] Hand = new Card[5];
    #endregion

    #region Extension Methods
    private static List<T> Shuffle<T>(List<T> list) {
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
}