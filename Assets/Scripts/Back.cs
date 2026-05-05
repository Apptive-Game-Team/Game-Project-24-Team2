using UnityEngine;
using UnityEngine.SceneManagement;

public class Back : MonoBehaviour
{
    private BoxCollider2D backCollider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        backCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsClickBack())
        {
            SceneManager.LoadScene("Default");
        }
    }

    bool IsClickBack()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            return backCollider == Physics2D.OverlapPoint(mousePos); 
        }

        return false;
    }
}
