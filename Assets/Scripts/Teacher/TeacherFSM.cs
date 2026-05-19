using UnityEngine;
using System;

public class TeacherFSM : MonoBehaviour
{
    enum TeacherState
    {
        Writing,
        Prepare,
        Watching
    }

    public static event Action OnTeacherTookNotes;
    public static event Action OnTeacherGaveSignal;
    public static event Action OnTeacherLookedBack;


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
                Debug.Log($"[선생님 이벤트](OnTeacherTookNotes)");
                OnTeacherTookNotes?.Invoke(); 
                sr.sprite = writingSprite;
                stateDuration = UnityEngine.Random.Range(5f, 10f);
                break;

            case TeacherState.Prepare:
                Debug.Log($"[선생님 이벤트](OnTeacherGaveSignal)");
                OnTeacherGaveSignal?.Invoke();
                sr.sprite = prepareSprite;
                stateDuration = 1f;
                if (audioSource != null && warningSound != null)
                    audioSource.PlayOneShot(warningSound);
                break;

            case TeacherState.Watching:
                Debug.Log($"[선생님 이벤트](OnTeacherLookedBack)");
                OnTeacherLookedBack?.Invoke();
                sr.sprite = watchingSprite;
                stateDuration = UnityEngine.Random.Range(1f, 3f);
                break;
        }
    }

    public bool IsWatching()
    {
        return currentState == TeacherState.Watching; // 외부에서 감시 여부 확인용
    }
}