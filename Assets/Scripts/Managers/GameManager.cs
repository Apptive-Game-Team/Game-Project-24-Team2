using UnityEngine;

public class GameManager : MonoBehaviour
{
    // 어디서나 GameManager.Instance로 접근할 수 있게 싱글톤 세팅
    public static GameManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // 칠판에서 5획이 다 차면 이 함수를 호출
    public void OnGameOver()
    {
        Debug.LogWarning("[GameManager] 게임 오버");
    }
}