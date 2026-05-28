using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryIconHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Hover Effect")]
    [Tooltip("이미지가 바뀔 Image 컴포넌트입니다. 비워두면 현재 게임 오브젝트의 Image를 사용합니다.")]
    [SerializeField] private Image _iconImage;
    [SerializeField] private Sprite _hoverSprite;

    private Sprite _defaultSprite;

    private void Awake()
    {
        if (_iconImage == null)
        {
            _iconImage = GetComponent<Image>();
        }

        if (_iconImage != null)
        {
            _defaultSprite = _iconImage.sprite;
        }
    }

    private void OnDisable()
    {
        // 오브젝트가 비활성화(인벤토리 열림)될 때 원래 스프라이트로 복구
        if (_iconImage != null && _defaultSprite != null)
        {
            _iconImage.sprite = _defaultSprite;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_iconImage != null && _hoverSprite != null)
        {
            _iconImage.sprite = _hoverSprite;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_iconImage != null && _defaultSprite != null)
        {
            _iconImage.sprite = _defaultSprite;
        }
    }
}