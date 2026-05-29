using UnityEngine;
using TMPro;

public class MoneyHUD : MonoBehaviour
{
    [Header("UI References")]
    [Tooltip("현재 금액을 표시할 TextMeshPro 컴포넌트입니다.")]
    [SerializeField] private TextMeshProUGUI moneyText;

    private void OnEnable()
    {
        // 돈이 변경될 때마다 UI를 업데이트하도록 이벤트 구독
        Money.OnMoneyChanged += UpdateMoneyUI;
        
        // 스크립트가 처음 켜질 때 현재 소지 금액으로 UI 초기화
        UpdateMoneyUI(Money.currentMoney);
    }

    private void OnDisable()
    {
        Money.OnMoneyChanged -= UpdateMoneyUI;
    }

    private void UpdateMoneyUI(float currentMoney)
    {
        if (moneyText != null)
        {
            // N0 포맷을 사용하여 천 단위 콤마 추가 (예: 1500 -> 1,500)
            moneyText.text = currentMoney.ToString("N0");
        }
    }
}