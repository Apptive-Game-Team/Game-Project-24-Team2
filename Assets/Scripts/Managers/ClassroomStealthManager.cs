using UnityEngine;
using UnityEngine.SceneManagement;

public class ClassroomStealthManager : MonoBehaviour
{
    public static ClassroomStealthManager Instance { get; private set; }

    public enum PlayerLocation
    {
        MainScene,
        DrawerScene
    }

    [Header("Player State")]
    [SerializeField] private PlayerLocation currentProperty = PlayerLocation.MainScene;

    private float _caughtCooldown = 0f;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 프리팹으로 메인씬 배치 후 씬 전환 시 파괴 방지
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // 씬 이름에 따라 플레이어 위치 상태 자동 변경
        if (scene.name == "MainScene")
        {
            ChangeLocation(PlayerLocation.MainScene);
        }
        else if (scene.name == "GrowingTestScene")
        {
            ChangeLocation(PlayerLocation.DrawerScene);
        }
    }

    // 플레이어가 이동할 때 위치를 바꿔주는 메소드 
    public void ChangeLocation(PlayerLocation newLocation)
    {
        currentProperty = newLocation;
        Debug.Log($"플레이어 위치 변경 -> {newLocation}");
    }

    private void Update()
    {
        if (_caughtCooldown > 0f) _caughtCooldown -= Time.deltaTime;

        // 선생님이 감시 중인데 플레이어가 서랍씬에 머물러 있다면 지속적으로 적발
        if (currentProperty == PlayerLocation.DrawerScene && TeacherManager.Instance != null && TeacherManager.Instance.IsWatching())
        {
            if (_caughtCooldown <= 0f)
            {
                Debug.LogWarning("서랍 안(딴짓)에 머무르다 선생님에게 들켰습니다!");
                if (StudentDamageHandler.Instance != null) StudentDamageHandler.Instance.HandleDamage();
                _caughtCooldown = 1.0f; // 1초에 한 번만 데미지를 입도록 설정
            }
        }
    }
}