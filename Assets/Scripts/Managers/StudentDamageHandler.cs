using UnityEngine;
using System;
using System.Collections;
using UnityEngine.SceneManagement;

public class StudentDamageHandler : MonoBehaviour
{
    [Header("Sound Settings")]
    private AudioSource audioSource;
    [SerializeField] private AudioClip damageSound;

    // 💡 피격 시 발생할 이벤트 (DamageEffect 등에서 구독하여 사용)
    public static event Action OnPlayerDamaged;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetStaticVariables()
    {
        OnPlayerDamaged = null;
    }

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    // 플레이어가 데미지를 입을 시 발생하는 모든 이벤트들을 처리하는 메소드
    public void HandleDamage()
    {
        ExecuteDamageLogic(); // 데미지 및 피격 이펙트 즉시 실행

        if (SceneManager.GetActiveScene().name != "MainScene")
        {
            StartCoroutine(DelayedReturnToMainScene());
        }
    }

    private IEnumerator DelayedReturnToMainScene()
    {
        // 1초 대기 후 메인씬으로 전환
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene("MainScene");
    }

    // 실제 데미지와 이펙트를 적용하는 로직
    private void ExecuteDamageLogic()
    {
        // 피격 이벤트 발생 (UI 이펙트 등 독립된 컴포넌트들에게 알림)
        OnPlayerDamaged?.Invoke();

        // 데미지 사운드 재생
        if (audioSource != null && damageSound != null)
        {
            audioSource.PlayOneShot(damageSound);
        }

        if (BlackboardLife.Instance != null)
        {
            BlackboardLife.Instance.GetDamage();
        }
        else
        {
            // 싱글톤이 없을 경우를 대비한 방어 코드 (직접 찾기)
            BlackboardLife blackboard = FindAnyObjectByType<BlackboardLife>();
            if (blackboard != null)
            {
                blackboard.GetDamage();
            }
            else
            {
                Debug.LogError("씬에 BlackboardLife 오브젝트가 없습니다!");
            }
        }
    }
}