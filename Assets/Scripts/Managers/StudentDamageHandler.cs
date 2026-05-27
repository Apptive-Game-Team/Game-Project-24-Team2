using UnityEngine;

public class StudentDamageHandler : MonoBehaviour
{
    [Header("Sound Settings")]
    private AudioSource audioSource;
    [SerializeField] private AudioClip damageSound;

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