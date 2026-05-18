using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopItem : MonoBehaviour
{
    [Header("ShopItem UI 요소들")]
    [SerializeField] public Image iconImage;
    [SerializeField] public TextMeshProUGUI nameText;
    [SerializeField] public TextMeshProUGUI priceText;
    [SerializeField] public Button buyButton;

    public void SetData(Item item)
    {
        iconImage.sprite = item.Icon;
        nameText.text = item.ItemName;
        priceText.text = $"{item.Price} 원";
    }
}