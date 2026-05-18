using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragItemButton : MonoBehaviour,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler,
    IDropHandler
{
    [SerializeField] private Item _item;

    public Item Item => _item;

    private GameObject _dragIcon;
    private Canvas _canvas;

    private void Start()
    {
        // 부모 Canvas 가져오기
        _canvas = GetComponentInParent<Canvas>();
    }

    // 드래그 시작
    public void OnBeginDrag(PointerEventData eventData)
    {
        // 드래그 아이콘 생성
        _dragIcon = new GameObject("DragIcon");

        _dragIcon.transform.SetParent(
            _canvas.transform,
            false
        );

        // 드래그 아이콘 이미지 추가
        Image dragIconImage =
            _dragIcon.AddComponent<Image>();

        dragIconImage.raycastTarget = false;

        // 현재 아이템 아이콘 설정
        dragIconImage.sprite = _item.Icon;

        // 드래그 아이콘 크기 설정
        RectTransform rectTransform =
            _dragIcon.GetComponent<RectTransform>();

        rectTransform.sizeDelta = new Vector2(100, 100);

        // 마우스 위치로 이동
        _dragIcon.transform.position = eventData.position;
    }

    // 드래그 중
    public void OnDrag(PointerEventData eventData)
    {
        if (_dragIcon == null)
        {
            return;
        }

        // 마우스 따라 이동
        _dragIcon.transform.position = eventData.position;
    }

    // 드래그 종료
    public void OnEndDrag(PointerEventData eventData)
    {
        if (_dragIcon == null)
        {
            return;
        }

        // 드래그 아이콘 삭제
        Destroy(_dragIcon);
    }

    // 다른 아이템 위에 드랍
    public void OnDrop(PointerEventData eventData)
    {
        DragItemButton draggedButton =
            eventData.pointerDrag.GetComponent<DragItemButton>();

        if (draggedButton == null)
        {
            return;
        }

        // 아이템 조합 시도
        InventoryManager.Instance.TryCombine(
            draggedButton.Item,
            _item
        );
    }
}