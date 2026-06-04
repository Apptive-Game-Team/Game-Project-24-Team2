using UnityEngine;
using UnityEngine.SceneManagement;

public class ActiveCafeteriaButton : MonoBehaviour
{
    private static ActiveCafeteriaButton _instance;

    [SerializeField] private GameObject _cafeteriaButton;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
        _cafeteriaButton.SetActive(false);
        InventoryManager.OnMealMushroomReaped += ActiveGoToCafeteriaButton;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "CafeteriaScene")
        {
            gameObject.SetActive(false);
        }
        if (scene.name != "CafeteriaScene")
        {
            GameObject cafeteriaCanvas = GameObject.Find("CafeteriaCanvas");
            if (cafeteriaCanvas != null)
            {
                cafeteriaCanvas.SetActive(true);
            }
        }
    }

    private void OnDestroy()
    {
        if (_instance == this)
        {
            InventoryManager.OnMealMushroomReaped -= ActiveGoToCafeteriaButton;
            SceneManager.sceneLoaded -= OnSceneLoaded;
            _instance = null;
        }
    }

    private void ActiveGoToCafeteriaButton()
    {
        _cafeteriaButton.SetActive(true);
        Debug.Log("재배됐나요?");
    }
}
