using UnityEngine;

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
        // OnTeacherLookedBack 이벤트를 수신
        TeacherFSM.OnTeacherLookedBack += CheckDamageCondition;
    }

    private void OnDisable()
    {
        TeacherFSM.OnTeacherLookedBack -= CheckDamageCondition;
    }

    // 데미지 조건을 만족하는지 체크하고 StudentDamageHandler를 실행시키는 메서드
    private void CheckDamageCondition()
    {
        // 데미지 조건 : 현재 플레이어가 서랍씬일 경우
        if (currentProperty == PlayerLocation.DrawerScene)
        {
            Debug.LogWarning("데미지 조건 만족!");

            // 씬에 배치된 StudentDamageHandler를 찾아서 HandleDamage 메소드 실행
            StudentDamageHandler damageHandler = FindAnyObjectByType<StudentDamageHandler>();
            if (damageHandler != null)
            {
                damageHandler.HandleDamage();
            }
            else
            {
                Debug.LogError("씬에 StudentDamageHandler 오브젝트가 배치되어 있지 않습니다!");
            }
        }
    }

    // 플레이어가 이동할 때 위치를 바꿔주는 메소드 (외부 호출용)
    public void ChangeLocation(PlayerLocation newLocation)
    {
        currentProperty = newLocation;
        Debug.Log($"플레이어 위치 변경 -> {newLocation}");
    }
}