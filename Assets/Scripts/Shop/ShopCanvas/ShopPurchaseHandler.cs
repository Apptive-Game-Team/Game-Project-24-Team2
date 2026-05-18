using UnityEngine;
using TMPro;
using System;

[RequireComponent(typeof(AudioSource))]
public class ShopPurchaseHandler : MonoBehaviour
{
    private TextMeshProUGUI moneyText;

    private int startingMoney = 50000;
    private int currentMoney;
    public static event Action<Item> OnItemPurchased;
    private AudioSource audioSource;
    private AudioClip purchaseCompleteSound;
    private AudioClip purchaseFailSound;

    private void Awake()
    {
        currentMoney = startingMoney;

        audioSource = GetComponent<AudioSource>();
        purchaseCompleteSound = Resources.Load<AudioClip>("Shop/ShopPurchaseCompleteSound1");
        purchaseFailSound = Resources.Load<AudioClip>("Shop/ShopPurchaseFailSound1");
    }

    public void SetMoneyText(TextMeshProUGUI ui)
    {
        moneyText = ui;
        UpdateMoneyUI(); // 초기 UI 업데이트
    }

    public void UpdateMoneyUI()
    {
        moneyText.text = $"보유금액 : {currentMoney}원";
    }

    public void TryPurchase(Item item)
    {
        if (currentMoney >= item.Price)
        {
            // 돈 차감
            currentMoney -= item.Price;
            UpdateMoneyUI();

            // 아이템 구매 이벤트 전파
            OnItemPurchased?.Invoke(item);

            // 구매 사운드 출력
            audioSource.PlayOneShot(purchaseCompleteSound);
        }
        else
        {
            // 구매 실패 사운드 출력
            audioSource.PlayOneShot(purchaseFailSound);
            
        }
    }
}