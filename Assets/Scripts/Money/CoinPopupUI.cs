using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.InputSystem;

public class CoinPopupUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject popupObject;
    [SerializeField] private TextMeshProUGUI amountText;

    private CanvasGroup popupCanvasGroup;

    [Header("Settings")]
    [SerializeField] private float displayDuration = 0.03f;
    [SerializeField] private float fadeDuration = 0.2f;
    [SerializeField] private float moveSpeed = 100f;

    private Coroutine disableCoroutine;
    private Vector3 initialPosition;

    private void Awake()
    {
        if (popupObject != null)
        {
            popupCanvasGroup = popupObject.GetComponent<CanvasGroup>();
            initialPosition = popupObject.transform.localPosition;
        }
    }

    private void OnEnable()
    {
        Money.OnMoneyEarned += ShowCoinPopup;
    }

    private void OnDisable()
    {
        Money.OnMoneyEarned -= ShowCoinPopup;
    }

    private void Update()
    {
        // C 키
        if (Keyboard.current.cKey.wasPressedThisFrame)
        {
            // 오직 팝업 UI 작동 테스트(실제 돈이 추가되진 않음)
            ShowCoinPopup(777); 
            Debug.Log("[UI 치트키] 코인 팝업 테스트를 실행합니다. (+777)");
        }
    }

    // Money 스크립트에서 벌어들인 액수(int)를 던져주면 이 메서드가 실행
    private void ShowCoinPopup(int amount)
    {
        if (amount <= 0) return;

        amountText.text = $"+{amount}";

        if (disableCoroutine != null)
        {
            StopCoroutine(disableCoroutine);
        }

        if (popupCanvasGroup != null) popupCanvasGroup.alpha = 1f;
        popupObject.transform.localPosition = initialPosition;

        popupObject.SetActive(true);
        disableCoroutine = StartCoroutine(PopupSequence());
    }

    private IEnumerator PopupSequence()
    {
        float elapsed = 0f;

        // displayDuration 동안 위로 떠오르며 유지
        while (elapsed < displayDuration)
        {
            elapsed += Time.deltaTime;
            popupObject.transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);
            yield return null;
        }

        // 이어서 Fade Out
        elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            
            if (popupCanvasGroup != null)
            {
                popupCanvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            }
            
            popupObject.transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);
            yield return null;
        }

        popupObject.SetActive(false);
        disableCoroutine = null;
    }
}