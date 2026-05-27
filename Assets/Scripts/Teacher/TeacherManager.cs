using UnityEngine;
using System;

public class TeacherManager : MonoBehaviour
{
    public static TeacherManager Instance { get; private set; }

    public enum TeacherState
    {
        Writing,
        Prepare,
        Watching
    }

    public static event Action OnTeacherTookNotes;
    public static event Action OnTeacherGaveSignal;
    public static event Action OnTeacherLookedBack;

    public TeacherState CurrentState { get; private set; } = TeacherState.Writing;

    [Header("Difficulty Settings")]
    public float minWritingTime = 5f;
    public float maxWritingTime = 10f;
    public float prepareTime = 1f;
    public float minWatchingTime = 5f;
    public float maxWatchingTime = 10f;

    private float timer = 0f;
    private float stateDuration = 0f;
    private bool isGameActive = false; // 타이머 작동 여부

    // 💡 유니티 엔진이 시작될 때 매니저 오브젝트를 자동으로 생성하여 유지시킵니다.
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        GameObject managerGO = new GameObject("TeacherManager");
        managerGO.AddComponent<TeacherManager>();
        DontDestroyOnLoad(managerGO);
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // 자동 시작 대신 외부에서 StartTeacher()를 호출하도록 할 수도 있습니다.
        // 현재는 씬 진입 시 바로 시작하도록 유지합니다.
        StartTeacher();
    }

    public void StartTeacher()
    {
        isGameActive = true;
        ChangeState(TeacherState.Writing);
    }

    public void ResumeTeacher()
    {
        isGameActive = true;
    }

    public void StopTeacher()
    {
        isGameActive = false;
    }

    private void Update()
    {
        if (!isGameActive) return; // 활성화 상태가 아니면 타이머 정지

        timer += Time.deltaTime;

        if (timer >= stateDuration)
        {
            switch (CurrentState)
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
        CurrentState = newState;
        timer = 0f;

        switch (newState)
        {
            case TeacherState.Writing:
                stateDuration = UnityEngine.Random.Range(minWritingTime, maxWritingTime);
                Debug.Log($"[선생님 이벤트](OnTeacherTookNotes)");
                OnTeacherTookNotes?.Invoke();
                break;

            case TeacherState.Prepare:
                stateDuration = prepareTime;
                Debug.Log($"[선생님 이벤트](OnTeacherGaveSignal)");
                OnTeacherGaveSignal?.Invoke();
                break;

            case TeacherState.Watching:
                stateDuration = UnityEngine.Random.Range(minWatchingTime, maxWatchingTime);
                Debug.Log($"[선생님 이벤트](OnTeacherLookedBack)");
                OnTeacherLookedBack?.Invoke();
                break;
        }
    }

    public bool IsWatching()
    {
        return CurrentState == TeacherState.Watching;
    }
}