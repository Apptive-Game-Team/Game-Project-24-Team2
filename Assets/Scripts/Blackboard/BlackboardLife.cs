using UnityEngine;

public class BlackboardLife : MonoBehaviour
{
    public static BlackboardLife Instance { get; private set; }
    
    [Header("Life Counter Settings")]
    [SerializeField] private GameObject[] jungSteps; // Jung1 ~ Jung5 배열
    
    private static int currentDamage = 0; // 씬을 넘나들며 공유할 정적 데미지 변수

    // 에디터에서 플레이 모드를 껐다 켤 때 데미지가 남아있는 것을 방지 (초기화)
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetStaticVariables()
    {
        currentDamage = 0;
    }

    void Awake()
    {
        // 싱글톤 인스턴스 할당 및 중복 방지
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // 씬이 로드될 때(씬 전환 시) 현재 누적된 데미지 개수만큼 획(UI)을 활성화하여 동기화
        for (int i = 0; i < jungSteps.Length; i++)
        {
            if (jungSteps[i] != null) jungSteps[i].SetActive(i < currentDamage);
        }
    }

    private void Start()
    {
        // 1번 이상 적발된 기록이 있다면 이름 작성 유지
        if (currentDamage > 0 && Blackboard.Instance != null)
        {
            Blackboard.Instance.SetNoisyPerson("송이");
        }
    }

    public void GetDamage()
    {
        // 5획 미만일 때만 로직 실행
        if (currentDamage < jungSteps.Length)
        {
            // 현재 순서에 해당하는 획 오브젝트를 활성화
            if (jungSteps[currentDamage] != null)
            {
                jungSteps[currentDamage].SetActive(true);
                currentDamage++;
                
                Debug.Log($"[ 목숨 차감 > 현재 획 수: {currentDamage} ]");
            }

            // 5획이 채워지면 게임 오버 처리
            if (currentDamage >= 5)
            {
                // GameManager가 존재하는지 체크 후 호출
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.OnGameOver(); 
                }
                else
                {
                    Debug.LogError("씬에 GameManager가 없습니다!");
                }
            }
        }
    }
}