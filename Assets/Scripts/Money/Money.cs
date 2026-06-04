using UnityEngine;
using System;
using UnityEngine.InputSystem;

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

    private void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        // 0번 키를 누르면 100만원 추가 치트 발동
        if (keyboard.digit0Key.wasPressedThisFrame)
        {
            int cheatAmount = 1000000;
            currentMoney += cheatAmount;
            OnMoneyChanged?.Invoke(currentMoney);
            OnMoneyEarned?.Invoke(cheatAmount);
        }
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