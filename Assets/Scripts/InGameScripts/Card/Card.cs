using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Card : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler,
    IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Card Data")]
    public CardData data;

    [HideInInspector] private Canvas canvas;
    private Image imageComponent;
    private Vector3 offset;

    [Header("Movement")]
    [SerializeField] private float moveSpeedLimit = 50f;

    [Header("Selection")]
    [HideInInspector] public bool selected = false;
    [SerializeField] public float selectionOffset = 50f;
    private float pointerDownTime;
    private float pointerUpTime;

    [Header("States")]
    [HideInInspector] public bool isHovering = false;
    [HideInInspector] public bool isDragging = false;
    [HideInInspector] public bool wasDragged = false;

    [Header("Events")]
    [HideInInspector] public UnityEvent<Card> PointerEnterEvent;
    [HideInInspector] public UnityEvent<Card> PointerExitEvent;
    [HideInInspector] public UnityEvent<Card, bool> PointerUpEvent;
    [HideInInspector] public UnityEvent<Card> PointerDownEvent;
    [HideInInspector] public UnityEvent<Card> BeginDragEvent;
    [HideInInspector] public UnityEvent<Card> EndDragEvent;
    [HideInInspector] public UnityEvent<Card, bool> SelectEvent;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        imageComponent = GetComponent<Image>();
        if (data != null && data.sprite != null)
        {
            imageComponent.sprite = data.sprite;
        }
    }

    private void Update()
    {
        ClampPosition();

        if (isDragging)
        {
            Vector2 targetPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) - offset;
            Vector2 direction = (targetPosition - (Vector2)transform.position).normalized;
            float distance = Vector2.Distance(transform.position, targetPosition);
            Vector2 velocity = direction * Mathf.Min(moveSpeedLimit, distance / Time.deltaTime);
            transform.Translate(velocity * Time.deltaTime);
        }
    }

    public void Setup(CardData cardData)
    {
        data = cardData;
        if (imageComponent != null)
            imageComponent.sprite = data.sprite;
    }

    private void ClampPosition()
    {
        Vector2 screenBounds = Camera.main.ScreenToWorldPoint(
            new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z)
        );
        Vector3 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, -screenBounds.x, screenBounds.x);
        clampedPosition.y = Mathf.Clamp(clampedPosition.y, -screenBounds.y, screenBounds.y);
        transform.position = new Vector3(clampedPosition.x, clampedPosition.y, 0);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        BeginDragEvent?.Invoke(this);
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        offset = mousePosition - (Vector2)transform.position;
        isDragging = true;
        wasDragged = true;
        canvas.GetComponent<GraphicRaycaster>().enabled = false;
        imageComponent.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData) { }

    public void OnEndDrag(PointerEventData eventData)
    {
        EndDragEvent?.Invoke(this);
        isDragging = false;
        canvas.GetComponent<GraphicRaycaster>().enabled = true;
        imageComponent.raycastTarget = true;

        // Tạo list để chứa kết quả raycast UI
        List<RaycastResult> results = new List<RaycastResult>();
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };
        EventSystem.current.RaycastAll(pointerData, results);

        foreach (var result in results)
        {
            if (result.gameObject.CompareTag("PlayZone"))
            {
                PlayCardZone playZone = result.gameObject.GetComponent<PlayCardZone>();
                if (playZone != null)
                {
                    playZone.PlayCard(this);
                    break;
                }
            }
        }

        StartCoroutine(ResetDragFlag());
    }

    private IEnumerator ResetDragFlag()
    {
        yield return new WaitForEndOfFrame();
        wasDragged = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        PointerEnterEvent?.Invoke(this);
        isHovering = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        PointerExitEvent?.Invoke(this);
        isHovering = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        PointerDownEvent?.Invoke(this);
        pointerDownTime = Time.time;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        pointerUpTime = Time.time;
        bool isLongPress = (pointerUpTime - pointerDownTime > 0.2f);
        PointerUpEvent?.Invoke(this, isLongPress);

        if (isLongPress || wasDragged)
            return;

        selected = !selected;
        SelectEvent?.Invoke(this, selected);

        if (selected)
            transform.localPosition += transform.up * selectionOffset;
        else
            transform.localPosition = Vector3.zero;
    }

    public void Deselect()
    {
        if (!selected) return;
        selected = false;
        transform.localPosition = Vector3.zero;
    }

    public int SiblingAmount()
    {
        return transform.parent.CompareTag("Slot") ? transform.parent.parent.childCount - 1 : 0;
    }

    public int ParentIndex()
    {
        return transform.parent.CompareTag("Slot") ? transform.parent.GetSiblingIndex() : 0;
    }

    public float NormalizedPosition()
    {
        if (!transform.parent.CompareTag("Slot")) return 0f;
        int index = ParentIndex();
        int total = transform.parent.parent.childCount - 1;
        return Mathf.InverseLerp(0, total, index);
    }
}
