using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image _icon;
    [SerializeField] private TMP_Text _countText;

    // 현재 슬롯 아이템
    private Item _item;

    // 현재 아이템 개수
    private int _count;

    public Item Item => _item;
    public int Count => _count;

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