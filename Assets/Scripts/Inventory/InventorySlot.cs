using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler,
    IDropHandler
{
    [Header("UI")]
    [SerializeField] private Image _icon;
    [SerializeField] private TMP_Text _countText;

    // 현재 슬롯 아이템
    private Item _item;

    // 현재 아이템 개수
    private int _count;
    private GameObject _dragIcon;
    private Canvas _canvas;

    public Item Item => _item;
    public int Count => _count;

    private void Start()
    {
        _canvas = GetComponentInParent<Canvas>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 빈 슬롯이면 드래그 불가
        if (_item == null)
        {
            return;
        }

        _dragIcon = new GameObject("DragIcon");
        _dragIcon.transform.SetParent(_canvas.transform, false);

        Image image = _dragIcon.AddComponent<Image>();

        image.sprite = _icon.sprite;
        image.raycastTarget = false;

        RectTransform rect =
            _dragIcon.GetComponent<RectTransform>();

        rect.sizeDelta = new Vector2(80, 80);

        _dragIcon.transform.position = eventData.position;
    }

    // 드래그 중 이동
    public void OnDrag(PointerEventData eventData)
    {
        if (_dragIcon == null)
        {
            return;
        }

        _dragIcon.transform.position = eventData.position;
    }

    // 드래그 종료
    public void OnEndDrag(PointerEventData eventData)
    {
        if (_dragIcon != null)
        {
            Destroy(_dragIcon);
        }
    }

    // 드롭 처리: 빈 슬롯은 이동, 찬 슬롯은 조합 시도
    public void OnDrop(PointerEventData eventData)
    {
        InventorySlot draggedSlot =
            eventData.pointerDrag.GetComponent<InventorySlot>();

        if (draggedSlot == null)
        {
            return;
        }

        if (draggedSlot == this)
        {
            return;
        }

        // 빈 슬롯이면 아이템 이동
        if (_item == null)
        {
            SetItem(draggedSlot.Item, draggedSlot.Count);
            draggedSlot.Clear();
            return;
        }

        // 아이템이 있으면 조합 시도
        InventoryManager.Instance.TryCombine(
            draggedSlot.Item,
            _item
        );
    }

    // 슬롯에 아이템 설정
    public void SetItem(Item item, int count)
    {
        _item = item;
        _count = count;

        UpdateUI();
    }

    // 아이템 개수 변경
    public void AddCount(int amount)
    {
        _count += amount;

        UpdateUI();
    }

    // 슬롯 UI 다시 갱신
    public void RefreshUI()
    {
        UpdateUI();
    }

    // 슬롯 비우기
    public void Clear()
    {
        _item = null;
        _count = 0;

        _icon.enabled = false;
        _countText.text = "";
    }

    // UI 갱신
    private void UpdateUI()
    {
        // 빈 슬롯이면 숨김
        if (_item == null)
        {
            _icon.enabled = false;
            _countText.text = "";

            return;
        }

        // 아이콘 표시
        _icon.enabled = true;
        _icon.sprite = _item.Icon;

        // 개수 표시
        _countText.text = _count.ToString();
    }
}