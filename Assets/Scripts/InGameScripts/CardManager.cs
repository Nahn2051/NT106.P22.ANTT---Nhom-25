using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public Sprite[] allCardSprites;
    public GameObject cardPrefab;  
    public Transform drawZone;
    public CardHolder CardHolder;

    private List<CardData> deck = new List<CardData>();
    private int[] cardQuantities = { 4, 6, 4, 4, 5, 4, 4, 5, 5 };

    private void Start()
    {
        CreateDeck();
        ShuffleDeck();
    }

    private void CreateDeck()
    {
        int count = 0;
        int index = 0;
        AddCards("Exploding", ref count, cardQuantities[index++]);
        AddCards("Defuse", ref count, cardQuantities[index++]);
        AddCards("Attack", ref count, cardQuantities[index++]);
        AddCards("Favor", ref count, cardQuantities[index++]);
        AddCards("Nope", ref count, cardQuantities[index++]);
        AddCards("Shuffle", ref count, cardQuantities[index++]);
        AddCards("Skip", ref count, cardQuantities[index++]);
        AddCards("SeeTheFuture", ref count, cardQuantities[index++]);

        for (int i = 0; i < cardQuantities[index]; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                AddCard("Normal", count + i);
            }
        }
    }

    private void AddCards(string name, ref int count, int quantity)
    {
        for (int i = 0; i < quantity; i++)
        {
            AddCard(name, count + i);
        }
        count += quantity;
    }

    private void AddCard(string name, int spriteIndex)
    {
        if (spriteIndex >= allCardSprites.Length)
        {
            Debug.LogWarning("Sprite index out of range for card: " + name);
            return;
        }
        CardData data = new CardData
        {
            cardName = $"{name}_{spriteIndex}",
            sprite = allCardSprites[spriteIndex],
            effect = name
        };
        deck.Add(data);
    }

    private void ShuffleDeck()
    {
        deck = deck.OrderBy(a => Random.value).ToList();
    }

    public void OnDrawButtonClick()
    {
        if (deck.Count == 0) return;
        CardData data = deck[0];
        deck.RemoveAt(0);

        CardHolder.DrawCard(cardPrefab, data);
    }
}
