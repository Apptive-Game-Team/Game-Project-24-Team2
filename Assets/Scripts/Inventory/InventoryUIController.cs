using UnityEngine;

public class InventoryUIController : MonoBehaviour
{
    public GameObject inventoryPanel;
    public GameObject openButton;

    public void OpenInventory()
    {
        inventoryPanel.SetActive(true);
        openButton.SetActive(false);
    }

    public void CloseInventory()
    {
        inventoryPanel.SetActive(false);
        openButton.SetActive(true);
    }
}