using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    // 게임 시작 버튼을 누르면 호출될 함수
    public void ClickStartButton()
    {
        SceneManager.LoadScene("MainScene"); 
    }

    // 게임 화면에서 타이틀 화면으로 돌아가는 버튼
    public void ClickTitleButton()
    {
        SceneManager.LoadScene("StartTestScene"); 
    }

    // 게임 종료 버튼을 누르면 호출될 함수
    public void ClickExitButton()
    {
        #if UNITY_EDITOR // 유니티 에디터에서 테스트할 때 꺼지도록 하는 코드
        UnityEditor.EditorApplication.isPlaying = false;
        #else // 실제 빌드된 게임(PC, 모바일 등)이 종료되는 코드
        Application.Quit();
        #endif
    }
}