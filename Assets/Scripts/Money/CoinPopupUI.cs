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

    [Header("Positioning")]
    [Tooltip("팝업이 나타날 기준 위치(예: MoneyHUD의 Transform)입니다. 비워두면 원래 위치에서 나타납니다.")]
    [SerializeField] private Transform targetTransform;
    [SerializeField] private Vector3 spawnOffset = new Vector3(-50f, -50f, 0f);

    private Coroutine disableCoroutine;
    private Vector3 initialPosition;

    [Header("Sound")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip popupSound;

    private void Awake()
    {
        if (popupObject != null)
        {
            popupCanvasGroup = popupObject.GetComponent<CanvasGroup>();
            initialPosition = popupObject.transform.localPosition;
        }

        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
            }
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

        if (audioSource != null && popupSound != null)
        {
            audioSource.PlayOneShot(popupSound);
        }

        amountText.text = $"+{amount}";

        if (disableCoroutine != null)
        {
            StopCoroutine(disableCoroutine);
        }

        if (popupCanvasGroup != null) popupCanvasGroup.alpha = 1f;
        
        if (targetTransform != null)
        {
            // 기준점이 등록되어 있으면 해당 월드 위치로 이동 후 로컬 오프셋 적용
            popupObject.transform.position = targetTransform.position;
            popupObject.transform.localPosition += spawnOffset;
        }
        else
        {
            popupObject.transform.localPosition = initialPosition;
        }

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