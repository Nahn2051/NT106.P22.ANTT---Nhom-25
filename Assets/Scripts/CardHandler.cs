using UnityEngine;
using UnityEngine.EventSystems;

public class CardDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Vector3 startPosition;
    private CanvasGroup canvasGroup;
    private RectTransform rectTransform;
    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rectTransform = GetComponent<RectTransform>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        GameObject nearestSlot = FindNearestSlot();
        if (nearestSlot != null)
        {
            transform.position = nearestSlot.transform.position;
        }
        else
        {
            transform.position = startPosition; // Quay về vị trí cũ nếu không có chỗ gần nhất
        }

        canvasGroup.blocksRaycasts = true;
    }

    private GameObject FindNearestSlot()
    {
        GameObject[] slots = GameObject.FindGameObjectsWithTag("CardSlot");
        GameObject nearest = null;
        float minDistance = Mathf.Infinity;

        foreach (var slot in slots)
        {
            float dist = Vector3.Distance(transform.position, slot.transform.position);
            if (dist < minDistance)
            {
                minDistance = dist;
                nearest = slot;
            }
        }

        return nearest;
    }
}