using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    private static bool _isShopSubscribed;
    private static readonly List<Item> _pendingPurchases = new List<Item>();

    [Header("Items")]
    [SerializeField] private Item _textbookMushroom;
    [SerializeField] private Item _blackboardMushroom;
    [SerializeField] private Item _mealSpore;

    [Header("Sound")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _successClip;

    [Header("Effects")]
    [SerializeField] private GameObject _combineEffectPrefab;
    [SerializeField] private Transform _effectSpawnPoint;
    
    [Header("Slots")]
    [SerializeField] private InventorySlot[] _slots;


    // 게임 시작 시 상점 구매 이벤트 구독
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void InitializeShopSubscription()
    {
        if (_isShopSubscribed)
        {
            return;
        }

        ShopPurchaseHandler.OnItemPurchased += HandlePurchasedItem;
        _isShopSubscribed = true;
    }

    // 구매한 아이템을 처리하거나 보류 목록에 저장
    private static void HandlePurchasedItem(Item item)
    {
        if (Instance != null)
        {
            Instance.PutItem(item);
            return;
        }

        // InventoryManager가 아직 초기화되지 않았으면 보류 목록에 추가
        _pendingPurchases.Add(item);
    }

    // 초기화: 보류 중인 구매 아이템들을 처리
    private void Awake()
    {
        Instance = this;

        // 인벤토리 초기화 전에 구매된 아이템들을 모두 추가
        if (_pendingPurchases.Count > 0)
        {
            foreach (Item pending in _pendingPurchases)
            {
                PutItem(pending);
            }

            _pendingPurchases.Clear();
        }
    }

    private void Start()
    {
        // PutItem(_textbookMushroom);
        // PutItem(_textbookMushroom);
        // PutItem(_blackboardMushroom);
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }


    // 인벤토리에 아이템 추가: 같은 아이템 찾으면 개수 증가, 없으면 새 슬롯에 추가
    public void PutItem(Item item)
    {
        // 이미 있는 같은 아이템 검색
        foreach (InventorySlot slot in _slots)
        {
            if (slot.Item != null && AreSameItem(slot.Item, item))
            {
                slot.AddCount(1);
                Debug.Log(item.ItemName + " 개수 증가");
                return;
            }
        }

        // 빈 슬롯 검색
        foreach (InventorySlot slot in _slots)
        {
            if (slot.Item == null)
            {
                slot.SetItem(item, 1);
                Debug.Log(item.ItemName + " 새 슬롯 추가");
                RefreshAllSlots();
                return;
            }
        }

        Debug.Log("인벤토리가 가득 찼습니다.");
    }

    // 모든 슬롯의 UI를 다시 그리기 (닫혀있던 인벤토리 열 때 사용)
    public void RefreshAllSlots()
    {
        foreach (InventorySlot slot in _slots)
        {
            slot.RefreshUI();
        }
    }

    // 두 아이템이 같은 종류인지 비교 (참조 또는 이름+순서로)
    private bool AreSameItem(Item a, Item b)
    {
        if (a == null || b == null)
        {
            return false;
        }

        // 같은 참조이거나 이름과 순서가 같으면 같은 아이템
        return a == b || (a.ItemName == b.ItemName && a.ShopOrder == b.ShopOrder);
    }

    // 특정 아이템 개수 반환
    private int GetItemCount(Item item)
    {
        foreach (InventorySlot slot in _slots)
        {
            if (slot.Item != null && AreSameItem(slot.Item, item))
            {
                return slot.Count;
            }
        }

        return 0;
    }

    // 아이템 개수 감소
    private void RemoveItem(Item item, int amount)
    {
        foreach (InventorySlot slot in _slots)
        {
            if (slot.Item != null && AreSameItem(slot.Item, item))
            {
                slot.AddCount(-amount);

                // 개수가 0 이하이면 슬롯 비우기
                if (slot.Count <= 0)
                {
                    slot.Clear();
                }

                return;
            }
        }
    }

    // 아이템 조합 시도
    public void TryCombine(Item draggedItem, Item targetItem)
    {
        // 교과서 버섯 + 칠판 버섯 조합 검사
        bool isCorrectCombination =
            (draggedItem == _textbookMushroom &&
             targetItem == _blackboardMushroom)
            ||
            (draggedItem == _blackboardMushroom &&
             targetItem == _textbookMushroom);

        if (!isCorrectCombination)
        {
            Debug.Log("조합 실패");
            return;
        }

        // 재료 부족 검사
        if (GetItemCount(_textbookMushroom) < 1)
        {
            Debug.Log("교과서 버섯 부족");
            return;
        }

        if (GetItemCount(_blackboardMushroom) < 1)
        {
            Debug.Log("칠판 버섯 부족");
            return;
        }

        // 재료 제거
        RemoveItem(_textbookMushroom, 1);
        RemoveItem(_blackboardMushroom, 1);

        // 결과 아이템 추가
        PutItem(_mealSpore);

        // 조합 성공 사운드 재생
        if (_audioSource != null && _successClip != null)
        {
            _audioSource.PlayOneShot(_successClip);
        }

        // 조합 성공 이펙트 생성
        if (_combineEffectPrefab != null)
        {
            GameObject effect = Instantiate(
                _combineEffectPrefab,
                _effectSpawnPoint.position,
                Quaternion.identity,
                _effectSpawnPoint
            );

            ParticleSystem ps =
                effect.GetComponent<ParticleSystem>();

            if (ps != null)
            {
                ps.Play();
            }
        }

        Debug.Log("조합 성공");
    }
}