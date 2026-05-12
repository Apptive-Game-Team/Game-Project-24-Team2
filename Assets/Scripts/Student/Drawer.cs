using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class Drawer : MonoBehaviour
{
    private BoxCollider2D drawerCollider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        drawerCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        if (IsClickDrawer())
        {
            LoadGrowingTestScene();
        }
    }

    private bool IsClickDrawer()
    {
        if (Mouse.current.leftButton.isPressed)
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

    private void LoadGrowingTestScene() //GrowingTestScene을 Hierarchy에 올리고 Unload한 후 실행
    {
        SceneManager.LoadScene("GrowingTestScene");
    }
}
