using UnityEngine;
using System.Collections.Generic;
using DG.Tweening;
using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine.UI;
using UnityEngine.UIElements; // cần có DOTween

public class PlayCardZone : MonoBehaviour
{
    public List<Card> playedCards = new List<Card>();
    public CardHolder handHolder;

    public void PlayCard(Card card)
    {
        if (playedCards.Contains(card)) return;

        playedCards.Add(card);
        card.isDragging = false;
        card.isHovering = false;
        card.selected = false;

        // Tắt kéo thả bằng cách tắt raycast và interaction
        card.GetComponent<UnityEngine.UI.Image>().raycastTarget = true;
        card.GetComponent<CanvasGroup>().blocksRaycasts = false;
        handHolder.RemoveCard(card);

        // Random xoay
        float randomAngle = Random.Range(-20f, 20f);

        // Set vị trí, cha mới
        card.transform.SetParent(transform);
        card.transform.SetAsLastSibling(); // Đặt lên trên cùng
        card.transform.localScale = Vector3.one * 0.4f;

        // Animation
        card.transform.DOLocalMove(Vector3.zero, 0.25f).SetEase(Ease.OutBack);
        card.transform.DORotate(new Vector3(0, 0, randomAngle), 0.25f);

        card.GetComponent<UnityEngine.UI.Image>().enabled = true;

        Debug.Log($"Kích hoạt hiệu ứng của {card.data.cardName}: {card.data.effect}");
    }
}
