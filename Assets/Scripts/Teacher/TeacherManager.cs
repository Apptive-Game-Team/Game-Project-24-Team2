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

    private float timer = 0f;
    private float stateDuration = 0f;

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
        ChangeState(TeacherState.Writing);
    }

    private void Update()
    {
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
                stateDuration = UnityEngine.Random.Range(5f, 10f);
                Debug.Log($"[선생님 이벤트](OnTeacherTookNotes)");
                OnTeacherTookNotes?.Invoke();
                break;

            case TeacherState.Prepare:
                stateDuration = 1f;
                Debug.Log($"[선생님 이벤트](OnTeacherGaveSignal)");
                OnTeacherGaveSignal?.Invoke();
                break;

            case TeacherState.Watching:
                stateDuration = UnityEngine.Random.Range(1f, 3f);
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