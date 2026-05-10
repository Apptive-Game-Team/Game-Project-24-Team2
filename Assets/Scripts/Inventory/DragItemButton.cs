using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragItemButton : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [SerializeField] private ItemType _itemType;

    private GameObject _dragIcon;
    private Canvas _canvas;

    private void Start()
    {
        _canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _dragIcon = new GameObject("DragIcon");
        _dragIcon.transform.SetParent(_canvas.transform, false);

        Image dragIconImage = _dragIcon.AddComponent<Image>();
        dragIconImage.raycastTarget = false;

        Image originalImage = GetComponent<Image>();

        if (originalImage != null)
        {
            dragIconImage.sprite = originalImage.sprite;
            dragIconImage.color = originalImage.color;
        }

        RectTransform rectTransform = _dragIcon.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(100, 100);

        _dragIcon.transform.position = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_dragIcon == null)
        {
            return;
        }

        _dragIcon.transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_dragIcon == null)
        {
            return;
        }

        Destroy(_dragIcon);
    }

    public void OnDrop(PointerEventData eventData)
    {
        DragItemButton draggedButton = eventData.pointerDrag.GetComponent<DragItemButton>();

        if (draggedButton == null)
        {
            return;
        }

        InventoryManager.Instance.TryCombine(draggedButton._itemType, _itemType);
    }
}