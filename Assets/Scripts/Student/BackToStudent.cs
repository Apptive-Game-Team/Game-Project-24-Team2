using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class BackToStudent : MonoBehaviour
{
    private BoxCollider2D _backCollider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _backCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsClickBack())
        {
            SceneManager.LoadScene("MainScene");
        }
    }

    bool IsClickBack()
    {
        if (Mouse.current.leftButton.isPressed)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

            Collider2D hit = Physics2D.OverlapPoint(mousePos);
            if (hit == _backCollider)
            {
                return true;
            }
        }

        return false;
    }
}
