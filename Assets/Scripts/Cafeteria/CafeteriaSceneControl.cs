using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using Unity.Collections;
using System;

public class CafeteriaSceneControl : MonoBehaviour
{
    [SerializeField] private GameObject _teacher1;
    [SerializeField] private GameObject _teacher2;
    [SerializeField] private GameObject _background;
    [SerializeField] private GameObject _black1;
    [SerializeField] private GameObject _black2;
    [SerializeField] private GameObject _meal;
    [SerializeField] private GameObject _table;
    [SerializeField] private GameObject _blackBackground;
    [SerializeField] private GameObject _finish;
    [SerializeField] private GameObject _inventory;

    [Header("Sound")]
    [SerializeField] private AudioSource _walkingSound;
    [SerializeField] private AudioSource _doom;
    [SerializeField] private AudioSource _finishSound;

    private void Awake()
    {
        _teacher2.SetActive(false);
        _meal.SetActive(false);
        _table.SetActive(false);
        _blackBackground.SetActive(false);
        _finish.SetActive(false);
        InventorySlot.OnDragEndedWorld += HandleMealMushroomDrop;
        StartCoroutine(MoveTeacher());
    }

    private void OnDestroy()
    {
        InventorySlot.OnDragEndedWorld -= HandleMealMushroomDrop;
    }
    private IEnumerator MoveTeacher()
    {
        _teacher1.SetActive(true);
        _teacher1.transform.position = new Vector3(0.5f, -1f, 0f);
        _background.transform.position = new Vector3(0f, 0f, 0f);
        SetScale(_background, 1.1f, 1.1f, 1f);
        SetScale(_black1, 18f, 1f, 1f);
        SetScale(_black2, 18f, 1f, 1f);
        yield return new WaitForSeconds(1.5f);

        SpriteRenderer teacher1Sprite = _teacher1.GetComponent<SpriteRenderer>();
        StartCoroutine(FadeOut(teacher1Sprite, 0.5f));
        _teacher2.SetActive(true);
        SpriteRenderer teacher2Sprite = _teacher2.GetComponent<SpriteRenderer>();
        StartCoroutine(FadeIn(teacher2Sprite, 0.5f));
        _walkingSound.Play();
        yield return new WaitForSeconds(2f);

        _teacher1.SetActive(false);
        StartCoroutine(FadeOut(teacher2Sprite, 0.5f));
        _walkingSound.Play();
        yield return new WaitForSeconds(1.5f);

        _teacher2.SetActive(false);
        _background.transform.position = new Vector3(-0.5f, 1f, 0f);
        SetScale(_background, 1.75f, 1.75f, 1f);
        SetScale(_black1, 18f, 3f, 1f);
        SetScale(_black2, 18f, 3f, 1f);
        _doom.Play();
        yield return new WaitForSeconds(0.2f);

        _background.transform.position = new Vector3(-1f, 2f, 0f);
        SetScale(_background, 2.5f, 2.5f, 1f);
        SetScale(_black1, 18f, 4f, 1f);
        SetScale(_black2, 18f, 4f, 1f);
        _doom.Play();
        yield return new WaitForSeconds(0.2f);

        _background.transform.position = new Vector3(-1.5f, 3f, 0f);
        SetScale(_background, 3.25f, 3.25f, 1f);
        SetScale(_black1, 18f, 5f, 1f);
        SetScale(_black2, 18f, 5f, 1f);
        _doom.Play();
        yield return new WaitForSeconds(0.2f);

        _background.transform.position = new Vector3(-2f, 4f, 0f);
        SetScale(_background, 4f, 4f, 1f);
        SetScale(_black1, 18f, 7f, 1f);
        SetScale(_black2, 18f, 7f, 1f);
        _doom.Play();
        yield return new WaitForSeconds(2f);

        _black1.SetActive(false);
        _black2.SetActive(false);

        _meal.SetActive(true);
        _table.SetActive(true);
    }

    private void SetScale(GameObject obj, float x, float y, float z)
    {
        obj.transform.localScale = new Vector3(x, y, z);
    }

    private IEnumerator FadeIn(SpriteRenderer spriteRenderer, float duration)
    {
        float elapsedTime = 0f;
        Color color = spriteRenderer.color;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Clamp01(elapsedTime / duration);
            spriteRenderer.color = color;
            yield return null;
        }

        color.a = 1f;
        spriteRenderer.color = color;
    }

    private IEnumerator FadeOut(SpriteRenderer spriteRenderer, float duration)
    {
        float elapsedTime = 0f;
        Color color = spriteRenderer.color;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Clamp01(1f - (elapsedTime / duration));
            spriteRenderer.color = color;
            yield return null;
        }

        color.a = 0f;
        spriteRenderer.color = color;
    }

    private void HandleMealMushroomDrop(Item item, Vector2 mousePos, Action<bool> onResult)
    {
        GameObject inventoryCanvas = GameObject.Find("InventoryCanvas");
        if (inventoryCanvas != null)
        {
            inventoryCanvas.SetActive(true);
        }

        if (_meal == null)
        {
            onResult?.Invoke(false);
            return;
        }

        Collider2D hit = Physics2D.OverlapPoint(mousePos);
        if (hit != null && hit.gameObject == _meal)
        {
            if (item.ItemName == "급식 버섯")
            {
                StartCoroutine(FinishScene());

                onResult?.Invoke(true);
            }
            else
            {
                onResult?.Invoke(false);
            }
        }
        else
        {
            onResult?.Invoke(false);
        }
    }
    
    private IEnumerator FinishScene()
    {
        // Inspector에서 할당된 인벤토리가 파괴되지 않았다면 비활성화
        if (_inventory != null)
        {
            _inventory.SetActive(false);
        }
        // DontDestroyOnLoad로 넘어와 살아있는 실제 인벤토리를 찾아 비활성화
        GameObject inventoryCanvas = GameObject.Find("InventoryCanvas");
        if (inventoryCanvas != null)
        {
            inventoryCanvas.SetActive(false);
        }
        _blackBackground.SetActive(true);

        SpriteRenderer tableSprite = _table.GetComponent<SpriteRenderer>();
        SpriteRenderer mealSprite = _meal.GetComponent<SpriteRenderer>();
        StartCoroutine(FadeOut(tableSprite, 0.6f));
        StartCoroutine(FadeOut(mealSprite, 0.6f));
        SpriteRenderer blackBackgroundSprite = _blackBackground.GetComponent<SpriteRenderer>();
        StartCoroutine(FadeIn(blackBackgroundSprite, 0.6f));
        yield return new WaitForSeconds(0.6f);

        _finish.SetActive(true);
        _finishSound.Play();
        SpriteRenderer finishSprite = _finish.GetComponent<SpriteRenderer>();
        StartCoroutine(FadeIn(finishSprite, 1f));
    }
}
