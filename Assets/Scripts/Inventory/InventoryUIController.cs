using UnityEngine;

public class InventoryUIController : MonoBehaviour
{
    [SerializeField] private GameObject _inventoryPanel;
    [SerializeField] private GameObject _openButton;

    public void OpenInventory()
    {
        _inventoryPanel.SetActive(true);
        _openButton.SetActive(false);
    }

    public void CloseInventory()
    {
        _inventoryPanel.SetActive(false);
        _openButton.SetActive(true);
    }
}