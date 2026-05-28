using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class Drawer : MonoBehaviour
{
    private BoxCollider2D drawerCollider;

    private void Start()
    {
        drawerCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        if (IsClickDrawer())
        {
            // 선생님이 감시 중일 때 서랍을 열려고 시도하면 적발
            if (TeacherManager.Instance != null && TeacherManager.Instance.IsWatching())
            {
                Debug.LogWarning("선생님에게 서랍 조작(딴짓)을 들켰습니다!");
                if (StudentDamageHandler.Instance != null) StudentDamageHandler.Instance.HandleDamage();
            }

            LoadGrowingTestScene();
        }
    }

    private bool IsClickDrawer()
    {
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
        {
            return false;
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

            Collider2D hit = Physics2D.OverlapPoint(mousePos);
            
            if (hit == drawerCollider)
            {
                return true;
            }
        }

        return false;
    }

    private void LoadGrowingTestScene()
    {
        SceneManager.LoadScene("GrowingTestScene");
    }
}
