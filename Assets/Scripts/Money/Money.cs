using UnityEngine;
using System;

public class Money : MonoBehaviour
{
    public static event Action<float> OnMoneyChanged;
    public static float currentMoney = 0f;

    private void OnEnable()
    {
        ShopPurchaseHandler.OnItemPurchased += HandleItemPurchased;
        Pot.OnMushroomReaped += HandleMushroomReaped;
        Book.OnStudyCompleted += HandleStudyCompleted;
    }

    private void OnDisable()
    {
        ShopPurchaseHandler.OnItemPurchased -= HandleItemPurchased;
        Pot.OnMushroomReaped -= HandleMushroomReaped;
        Book.OnStudyCompleted -= HandleStudyCompleted;
    }

    private void HandleItemPurchased(Item item)
    {
        currentMoney -= item.Price;
        OnMoneyChanged?.Invoke(currentMoney);
    }

    private void HandleMushroomReaped(Item mushroomItem)
    {
        currentMoney += 500f;
        OnMoneyChanged?.Invoke(currentMoney);
        Debug.Log("HandleMushroomReaped");
    }

    private void HandleStudyCompleted()
    {
        currentMoney += 50f;
        OnMoneyChanged?.Invoke(currentMoney);
        Debug.Log("HandleStudyCompleted");
    }
}