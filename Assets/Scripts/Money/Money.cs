using UnityEngine;
using System;

public class Money : MonoBehaviour
{
    public static event Action<float> OnMoneyChanged;
    public static event Action<int> OnMoneyEarned;
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
        int earned = 500;
        currentMoney += earned;
        OnMoneyChanged?.Invoke(currentMoney);
        OnMoneyEarned?.Invoke(earned); 
        Debug.Log("HandleMushroomReaped");
    }

    private void HandleStudyCompleted() 
    {
        int earnedMoney = 50;
        currentMoney += earnedMoney;
        OnMoneyChanged?.Invoke(currentMoney);
        OnMoneyEarned?.Invoke(earnedMoney);
        Debug.Log("HandleStudyCompleted");
    }
}