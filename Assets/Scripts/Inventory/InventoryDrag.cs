using UnityEngine;
using UnityEngine.EventSystems;

public class InventoryDrag : MonoBehaviour,
    IPointerDownHandler,
    IDragHandler
{
    private RectTransform _rectTransform;
    private RectTransform _canvasRect;

    // 마우스 클릭 위치 보정값
    private Vector2 _offset;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();

        // 부모 Canvas 가져오기
        _canvasRect =
            GetComponentInParent<Canvas>()
            .GetComponent<RectTransform>();
    }

    // 드래그 시작 위치 계산
    public void OnPointerDown(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _rectTransform,
            eventData.position,
            eventData.pressEventCamera,
            out _offset
        );
    }

    // 드래그 중 이동 처리
    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localPoint;

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _canvasRect,
            eventData.position,
            eventData.pressEventCamera,
            out localPoint))
        {
            Vector2 targetPos = localPoint - _offset;

            float canvasWidth = _canvasRect.rect.width;
            float canvasHeight = _canvasRect.rect.height;

            float panelWidth = _rectTransform.rect.width;
            float panelHeight = _rectTransform.rect.height;

            // 화면 밖으로 못 나가게 제한
            float minX = -canvasWidth / 2 + panelWidth / 2;
            float maxX = canvasWidth / 2 - panelWidth / 2;

            float minY = -canvasHeight / 2 + panelHeight / 2;
            float maxY = canvasHeight / 2 - panelHeight / 2;

            targetPos.x = Mathf.Clamp(targetPos.x, minX, maxX);
            targetPos.y = Mathf.Clamp(targetPos.y, minY, maxY);

            _rectTransform.localPosition = targetPos;
        }
    }
}