using UnityEngine;

public class TeacherFSM : MonoBehaviour
{
    enum TeacherState
    {
        Writing,
        Prepare,
        Watching
    }

    TeacherState currentState;

    [SerializeField] private Sprite writingSprite;
    [SerializeField] private Sprite prepareSprite;
    [SerializeField] private Sprite watchingSprite;
    [SerializeField] private AudioClip warningSound;

    SpriteRenderer sr;
    AudioSource audioSource;
    float timer = 0f;
    float stateDuration = 0f;

    private void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
        ChangeState(TeacherState.Writing);
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= stateDuration)
        {
            // 현재 상태에 따라 다음 상태로 변경
            switch (currentState)
            {
                case TeacherState.Writing:
                    ChangeState(TeacherState.Prepare);
                    break;
                case TeacherState.Prepare:
                    ChangeState(TeacherState.Watching);
                    break;
                case TeacherState.Watching:
                    ChangeState(TeacherState.Writing);
                    break;
            }
        }
    }

    private void ChangeState(TeacherState newState)
    {
        currentState = newState;
        timer = 0f;

        // 상태에 따른 스프라이트, 지속시간 및 효과음 설정
        switch (newState)
        {
            case TeacherState.Writing:
                sr.sprite = writingSprite;
                stateDuration = Random.Range(5f, 10f);
                break;

            case TeacherState.Prepare:
                sr.sprite = prepareSprite;
                stateDuration = 1f;
                if (audioSource != null && warningSound != null)
                    audioSource.PlayOneShot(warningSound);
                break;

            case TeacherState.Watching:
                sr.sprite = watchingSprite;
                stateDuration = Random.Range(1f, 3f);
                break;
        }
    }

    public bool IsWatching()
    {
        return currentState == TeacherState.Watching; // 외부에서 감시 여부 확인용
    }
}