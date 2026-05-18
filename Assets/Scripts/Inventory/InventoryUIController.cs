using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryUIController : MonoBehaviour
{
    [SerializeField] private GameObject _inventoryPanel;
    [SerializeField] private GameObject _openButton;

    // 인벤토리 열기: 닫혀있던 동안 추가된 아이템들을 UI에 모두 반영
    public void OpenInventory()
    {
        _inventoryPanel.SetActive(true);
        _openButton.SetActive(false);

        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.RefreshAllSlots();
        }
    }

    // 인벤토리 닫기
    public void CloseInventory()
    {
        _inventoryPanel.SetActive(false);
        _openButton.SetActive(true);
    }

    private void Update()
    {
        // E 키로 인벤토리 열기/닫기
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            bool isActive = _inventoryPanel.activeSelf;
            bool willOpen = !isActive;

            _inventoryPanel.SetActive(willOpen);
            _openButton.SetActive(isActive);

            // 열 때 닫혀있던 동안 추가된 아이템들도 반영
            if (willOpen && InventoryManager.Instance != null)
            {
                InventoryManager.Instance.RefreshAllSlots();
            }
        }
    }
}