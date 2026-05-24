using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InventorySlot : MonoBehaviour,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler,
    IDropHandler
{
    public static event Action<Item> OnDragStarted;
    public static event Action OnDragCanceled;
    public static event Action<Item, Vector2, Action<bool>> OnDragEndedWorld;

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

        _icon.enabled = false;
        _countText.text = "";
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

        // 스크린 좌표를 Canvas 로컬 좌표로 변환
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvas.GetComponent<RectTransform>(),
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPoint);

        rect.localPosition = localPoint;

        OnDragStarted?.Invoke(_item);
    }

    // 드래그 중 이동
    public void OnDrag(PointerEventData eventData)
    {
        if (_dragIcon == null)
        {
            return;
        }

        // 스크린 좌표를 Canvas 로컬 좌표로 변환
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvas.GetComponent<RectTransform>(),
            eventData.position,
            eventData.pressEventCamera,
            out Vector2 localPoint);

        _dragIcon.GetComponent<RectTransform>().localPosition = localPoint;
    }

    // 드래그 종료
    public void OnEndDrag(PointerEventData eventData)
    {
        if (_dragIcon != null)
        {
            Destroy(_dragIcon);
        }

        // 드래그 종료 시 월드에 이벤트 방송
        if (Camera.main != null && _item != null)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            OnDragEndedWorld?.Invoke(_item, mousePos, (success) => {
                if (success)
                {
                    AddCount(-1);
                    if (_count <= 0) Clear();
                }
            });
        }

        OnDragCanceled?.Invoke();
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