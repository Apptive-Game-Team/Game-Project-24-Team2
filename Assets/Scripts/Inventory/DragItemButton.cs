using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragItemButton : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    public ItemType itemType;

    private GameObject dragIcon;
    private Canvas canvas;

    private void Start()
    {
        canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        dragIcon = new GameObject("DragIcon");
        dragIcon.transform.SetParent(canvas.transform, false);

        Image iconImage = dragIcon.AddComponent<Image>();
        iconImage.raycastTarget = false;

        Image originalImage = GetComponent<Image>();
        if (originalImage != null)
        {
            iconImage.sprite = originalImage.sprite;
            iconImage.color = originalImage.color;
        }

        RectTransform rect = dragIcon.GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(100, 100);

        dragIcon.transform.position = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (dragIcon != null)
        {
            dragIcon.transform.position = eventData.position;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (dragIcon != null)
        {
            Destroy(dragIcon);
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        DragItemButton draggedButton = eventData.pointerDrag.GetComponent<DragItemButton>();

        if (draggedButton == null)
        {
            return;
        }

        InventoryManager.Instance.TryCombine(draggedButton.itemType, this.itemType);
    }
}