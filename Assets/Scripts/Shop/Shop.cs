using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(ShopManager))]
[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(SpriteRenderer))]

public class Shop : MonoBehaviour
{
    private ShopManager shopManager;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>(); // 상점 아이콘 표시
        shopManager = GetComponent<ShopManager>(); // 상점 기능
    }

    private void Update()
    {
        // 상점 클릭
        if (Mouse.current.leftButton.wasPressedThisFrame)
            HandleShopClick();
    }

    private void HandleShopClick()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {
            if (hit.collider.TryGetComponent<Shop>(out Shop shopIcon))
            {
                shopManager.OpenShop();
            }
        }
    }
}