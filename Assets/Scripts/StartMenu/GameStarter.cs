using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStarter : MonoBehaviour
{
    // 시작 버튼을 누르면 호출될 함수
    public void SceneChange()
    {
        SceneManager.LoadScene("MainScene");
    }
}