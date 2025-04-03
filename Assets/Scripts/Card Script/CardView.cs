//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;

//public class CardView : MonoBehaviour, IPointerClickHandler
//{
//    [SerializeField] private SpriteRenderer cardImage;
//    [SerializeField] private TMP_Text title;
//    [SerializeField] private TMP_Text cost;

//    private Card card;
//    public void Setup(Card card)
//    {
//        this.card = card;
//        cardImage.sprite = card.Sprite;
//        title.text = card.Title;
//        cost.text = card.Cost.ToString();
//    }
//    public void OnPointerClick(PointerEventData eventData)
//    {
//        card.PerformEffect();
//        Destroy(gameObject);
//    }
//}
