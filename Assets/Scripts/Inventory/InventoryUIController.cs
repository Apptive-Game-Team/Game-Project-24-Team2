using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryUIController : MonoBehaviour
{
    public static InventoryUIController Instance { get; private set; }

    [SerializeField] private GameObject _inventoryPanel;
    [SerializeField] private GameObject _openButton;

    [Header("Sound")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _openSound;
    [SerializeField] private AudioClip _closeSound;

    private void Awake()
    {
        // 씬을 이동할 때 캔버스가 중복으로 겹쳐서 버튼이 두 개씩 뜨는 현상을 방지합니다.
        if (Instance != null && Instance != this)
        {
            Destroy(transform.root.gameObject);
            return;
        }
        Instance = this;

        if (_audioSource == null)
        {
            _audioSource = GetComponent<AudioSource>();
            if (_audioSource == null)
            {
                _audioSource = gameObject.AddComponent<AudioSource>();
            }
        }
    }

    // 인벤토리 열기: 닫혀있던 동안 추가된 아이템들을 UI에 모두 반영
    public void OpenInventory()
    {
        _inventoryPanel.SetActive(true);
        _openButton.SetActive(false);

        if (_audioSource != null && _openSound != null)
        {
            _audioSource.PlayOneShot(_openSound);
        }

        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.RefreshAllSlots();
        }
    }

    // 인벤토리 닫기
    public void CloseInventory()
    {
        _inventoryPanel.SetActive(false);
        _openButton.SetActive(true);

        if (_audioSource != null && _closeSound != null)
        {
            _audioSource.PlayOneShot(_closeSound);
        }
    }

    private void Update()
    {
        // E 키로 인벤토리 열기/닫기
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            bool isActive = _inventoryPanel.activeSelf;

            if (!isActive)
            {
                OpenInventory();
            }
            else
            {
                CloseInventory();
            }
        }
    }
}