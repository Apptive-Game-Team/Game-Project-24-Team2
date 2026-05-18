using UnityEngine;
using UnityEngine.InputSystem;

public class BlackboardTest : MonoBehaviour
{
    [SerializeField] private Blackboard nameManager;
    [SerializeField] private BlackboardLife lifeManager;

    void Update()
    {
        // 키보드 장치가 연결되어 있는지 확인
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        // 1번 키를 누르면 이름 설정
        if (keyboard.digit1Key.wasPressedThisFrame)
        {
            if (nameManager != null)
            {
                nameManager.SetNoisyPerson("홍길동");
            }
        }

        // 2번 키를 누르면 데미지를 입히고 획 추가
        if (keyboard.digit2Key.wasPressedThisFrame)
        {
            if (lifeManager != null)
            {
                lifeManager.GetDamage();
            }
        }
    }
}