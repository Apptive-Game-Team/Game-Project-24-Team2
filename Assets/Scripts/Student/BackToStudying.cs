using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class BackToStudying : MonoBehaviour
{
    private BoxCollider2D backCollider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        backCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (IsClickBack())
        {
            SceneManager.LoadScene("StudyingTestScene");
        }
    }

    private bool IsClickBack()
    {
        if (Mouse.current.leftButton.isPressed)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

            Collider2D hit = Physics2D.OverlapPoint(mousePos);
            
            if (hit == backCollider)
            {
                return true;
            }
        }

        return false;
    }
}
