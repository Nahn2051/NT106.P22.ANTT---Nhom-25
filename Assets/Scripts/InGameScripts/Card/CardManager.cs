using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public Sprite[] allCardSprites;
    public GameObject cardPrefab;  
    public CardHolder cardHolder;
    public Transform cardDeckVisual;
    public static CardManager Instance;
    [SerializeField] private TMP_Text deckCardCount;

    private List<CardData> Deck = new List<CardData>();
    private int[] cardQuantities = { 4, 6, 4, 4, 5, 4, 4, 5, 5 };
    private void Awake()
    {
        // Đảm bảo chỉ có 1 instance của CardManager
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        CreateDeck();
        ShuffleDeck();
        UpdateDeckCount();
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
                AddCard($"Normal{i+1}", count + i, j + 1);
            }
        }
    }

    private void AddCards(string name, ref int count, int quantity)
    {
        for (int i = 0; i < quantity; i++)
        {
            AddCard(name, i + count, i + 1);
        }
        count += quantity;
    }

    private void AddCard(string name, int spriteIndex, int index)
    {
        if (spriteIndex >= allCardSprites.Length)
        {
            Debug.LogWarning("Sprite index out of range for card: " + name);
            return;
        }
        CardData data = new CardData
        {
            cardName = $"{name}_{index}",
            sprite = allCardSprites[spriteIndex],
            effect = name,
        };
        Deck.Add(data);
    }

    private void ShuffleDeck()
    {
        Deck = Deck.OrderBy(a => Random.value).ToList();
    }
    public void UpdateDeckCount()
    {
        if (deckCardCount != null)
        {
            deckCardCount.text = Deck.Count().ToString();
        }
    }

    public void OnDrawButtonClick()
    {
        if (Deck.Count == 0) return;
        CardData data = Deck[0];
        Deck.RemoveAt(0);
        UpdateDeckCount();
        CheckDeckVisual();
        cardHolder.DrawCard(cardPrefab, data);
    }

    public void CheckDeckVisual()
    {
        if (Deck.Count == 0)
        {
            Debug.Log("Deck is empty");
            cardDeckVisual.gameObject.SetActive(false);
            return;
        }
    }
}
