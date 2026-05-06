using UnityEngine;
using UnityEngine.SceneManagement;

public class Drawer : MonoBehaviour
{
    private BoxCollider2D drawerCollider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        drawerCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsClickDrawer())
        {
            LookDrawer();
        }
    }

    bool IsClickDrawer()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            return drawerCollider == Physics2D.OverlapPoint(mousePos);
        }

        return false;
    }

    void LookDrawer()
    {
        SceneManager.LoadScene("ClickDrawer");
    }
}
