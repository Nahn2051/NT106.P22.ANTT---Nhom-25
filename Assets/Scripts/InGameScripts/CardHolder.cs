using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;
using System;
using System.Collections;

public class CardHolder : MonoBehaviour
{
    private RectTransform rect;

    [HideInInspector] public List<Card> cards = new List<Card>();

    private Card selectedCard;
    private Card hoveredCard;
    private bool isCrossing = false;
    [SerializeField] private bool tweenCardReturn = true;

    private void Start()
    {
        rect = GetComponent<RectTransform>();
    }

    public void DrawCard(GameObject cardPrefab, CardData data)
    {
        if (cards.Count <= 6 && cards.Count != 0)
        {
            rect.sizeDelta = new Vector2(rect.sizeDelta.x + 204, rect.sizeDelta.y);
        }
        GameObject cardSlotObj = Instantiate(cardPrefab, transform);
        Card cardComponent = cardSlotObj.GetComponentInChildren<Card>();
        if (cardComponent == null)
        {
            Debug.LogError("Không tìm thấy component Card trong CardSlot prefab!");
            return;
        }

        cardComponent.Setup(data);
        cardSlotObj.transform.SetParent(transform, false);
        cards.Add(cardComponent);
        RegisterCardEvents(cardComponent);
        StartCoroutine(Frame());
        IEnumerator Frame()
        {
            yield return new WaitForSecondsRealtime(.1f);
        }
    }
    private void RegisterCardEvents(Card card)
    {
        card.PointerEnterEvent.AddListener(CardPointerEnter);
        card.PointerExitEvent.AddListener(CardPointerExit);
        card.BeginDragEvent.AddListener(BeginDrag);
        card.EndDragEvent.AddListener(EndDrag);
    }

    private void BeginDrag(Card card)
    {
        selectedCard = card;
    }


    void EndDrag(Card card)
    {
        if (selectedCard == null)
            return;

        selectedCard.transform.DOLocalMove(selectedCard.selected ? new Vector3(0, selectedCard.selectionOffset, 0) : Vector3.zero, tweenCardReturn ? .15f : 0).SetEase(Ease.OutBack);

        rect.sizeDelta += Vector2.right;
        rect.sizeDelta -= Vector2.right;

        selectedCard = null;

    }

    void CardPointerEnter(Card card)
    {
        hoveredCard = card;
    }

    void CardPointerExit(Card card)
    {
        hoveredCard = null;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Delete))
        {
            if (hoveredCard != null)
            {
                Destroy(hoveredCard.transform.parent.gameObject);
                cards.Remove(hoveredCard);

            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            foreach (Card card in cards)
            {
                card.Deselect();
            }
        }

        if (selectedCard == null)
            return;

        if (isCrossing)
            return;

        for (int i = 0; i < cards.Count; i++)
        {

            if (selectedCard.transform.position.x > cards[i].transform.position.x)
            {
                if (selectedCard.ParentIndex() < cards[i].ParentIndex())
                {
                    Swap(i);
                    break;
                }
            }

            if (selectedCard.transform.position.x < cards[i].transform.position.x)
            {
                if (selectedCard.ParentIndex() > cards[i].ParentIndex())
                {
                    Swap(i);
                    break;
                }
            }
        }
    }

    void Swap(int index)
    {
        isCrossing = true;

        Transform focusedParent = selectedCard.transform.parent;
        Transform crossedParent = cards[index].transform.parent;

        cards[index].transform.SetParent(focusedParent);
        cards[index].transform.localPosition = cards[index].selected ? new Vector3(0, cards[index].selectionOffset, 0) : Vector3.zero;
        selectedCard.transform.SetParent(crossedParent);

        isCrossing = false;
    }
    public void ShuffleButton()
    {
        cards = cards.OrderBy(a => UnityEngine.Random.value).ToList();
        for (int i = 0; i < cards.Count; i++)
        {
            cards[i].transform.parent.SetSiblingIndex(i);
        }
    }
}
