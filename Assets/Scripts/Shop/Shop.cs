using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

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
        // 인벤토리 등 UI 창 위에 마우스가 있을 때는 상점 클릭을 무시하여 관통을 방지합니다.
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

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