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

    public void SetData(Spore spore)
    {
        iconImage.sprite = spore.Icon;
        nameText.text = spore.SporeName;
        priceText.text = $"{spore.Price} 원";
    }
}