using UnityEngine;

public abstract class Item : ScriptableObject
{
    [SerializeField] private string _itemName = "아이템 이름";
    [SerializeField] private int _price = 0;
    [SerializeField] private Sprite _icon;
    [SerializeField] private int _shopOrder = -1;

    public string ItemName => _itemName;
    public int Price => _price;
    public Sprite Icon => _icon;
    public int ShopOrder => _shopOrder;
}