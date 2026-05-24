using UnityEngine;
using TMPro;
using System;

[RequireComponent(typeof(AudioSource))]
public class ShopPurchaseHandler : MonoBehaviour
{
    private TextMeshProUGUI moneyText;

    public static event Action<Item> OnItemPurchased;
    private AudioSource audioSource;
    private AudioClip purchaseCompleteSound;
    private AudioClip purchaseFailSound;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        purchaseCompleteSound = Resources.Load<AudioClip>("Shop/ShopPurchaseCompleteSound1");
        purchaseFailSound = Resources.Load<AudioClip>("Shop/ShopPurchaseFailSound1");
    }

    private void OnEnable()
    {
        Money.OnMoneyChanged += HandleMoneyChanged;
    }

    private void OnDisable()
    {
        Money.OnMoneyChanged -= HandleMoneyChanged;
    }

    public void SetMoneyText(TextMeshProUGUI ui)
    {
        moneyText = ui;
        HandleMoneyChanged(Money.currentMoney); // 초기 UI 업데이트
    }

    private void HandleMoneyChanged(float newMoney)
    {
        if (moneyText != null)
        {
            moneyText.text = $"보유금액 : {newMoney}원";
        }
    }

    public void TryPurchase(Item item)
    {
        if (Money.currentMoney >= item.Price)
        {
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