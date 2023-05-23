using UnityEngine;

public class Card {
    public string CardName { get; set; }
    public Sprite CardFace { get; set; }

    public Card() { }
    public Card(string cardName, Sprite cardFace) {
        CardName = cardName;
        CardFace = cardFace;
    }
}