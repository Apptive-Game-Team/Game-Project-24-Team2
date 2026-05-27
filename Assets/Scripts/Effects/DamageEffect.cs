using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DamageEffect : MonoBehaviour
{
    [Header("Effect Settings")]
    [Tooltip("피격 시 화면에 표시될 색상과 최대 투명도입니다.")]
    [SerializeField] private Color damageColor = new Color(1f, 0f, 0f, 0.4f); // 투명도 40%의 붉은색
    [Tooltip("화면이 다시 원래대로 돌아오는 속도입니다.")]
    [SerializeField] private float fadeSpeed = 2f;

    private Image overlayImage;
    private Coroutine flashCoroutine;

    private void Awake()
    {
        // 유니티 에디터에서 UI를 일일이 만들 필요 없이, 코드로 캔버스와 이미지를 자동 생성합니다.
        CreateAutoUI();
    }

    private void OnEnable()
    {
        // StudentDamageHandler의 피격 이벤트를 구독
        StudentDamageHandler.OnPlayerDamaged += TriggerFlash;
    }

    private void OnDisable()
    {
        // 이벤트 구독 해제 (메모리 누수 방지)
        StudentDamageHandler.OnPlayerDamaged -= TriggerFlash;
    }

    private void CreateAutoUI()
    {
        // 1. 최상단에 그려질 전용 캔버스 생성
        GameObject canvasObj = new GameObject("DamageEffectCanvas_AutoCreated");
        canvasObj.transform.SetParent(this.transform);

        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999; // 모든 UI를 덮도록 가장 높은 순서 지정

        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        // 2. 전체 화면을 덮는 이미지 생성
        GameObject imageObj = new GameObject("DamageOverlayImage");
        imageObj.transform.SetParent(canvasObj.transform, false);

        overlayImage = imageObj.AddComponent<Image>();
        overlayImage.color = new Color(damageColor.r, damageColor.g, damageColor.b, 0f); // 처음엔 투명하게
        overlayImage.raycastTarget = false; // 마우스 클릭을 방해하지 않도록 필수 설정!

        // 3. 이미지를 전체 화면 크기로 늘리기 (Stretch)
        RectTransform rect = overlayImage.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.sizeDelta = Vector2.zero;
        rect.anchoredPosition = Vector2.zero;
    }

    private void TriggerFlash()
    {
        if (flashCoroutine != null) StopCoroutine(flashCoroutine);
        flashCoroutine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        overlayImage.color = damageColor; // 가장 진한 빨간색으로 번쩍!
        while (overlayImage.color.a > 0f)
        {
            overlayImage.color = new Color(damageColor.r, damageColor.g, damageColor.b, overlayImage.color.a - (Time.deltaTime * fadeSpeed));
            yield return null;
        }
    }
}