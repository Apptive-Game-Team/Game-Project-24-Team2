using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryUIController : MonoBehaviour
{
    [SerializeField] private GameObject _inventoryPanel;
    [SerializeField] private GameObject _openButton;

    // 인벤토리 열기
    public void OpenInventory()
    {
        _inventoryPanel.SetActive(true);
        _openButton.SetActive(false);
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

            _inventoryPanel.SetActive(!isActive);
            _openButton.SetActive(isActive);
        }
    }
}