using UnityEngine;

public class BlackboardLife : MonoBehaviour
{
    [Header("Life Counter Settings")]
    [SerializeField] private GameObject[] jungSteps; // Jung1 ~ Jung5 배열
    
    private int currentDamage = 0; // 현재 감점 횟수 (0~5)

    void Awake()
    {
        // 게임 시작 시 모든 획을 보이지 않게 초기화
        foreach (GameObject step in jungSteps)
        {
            if (step != null) step.SetActive(false);
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
                GameOver();
            }
        }
    }

    private void GameOver() // 5획 채워졌을 때 실행
    {
        Debug.LogWarning("[ 게임 오버 ]");
    }
}