using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    private static bool _isShopSubscribed;
    private static readonly List<Item> _pendingPurchases = new List<Item>();

    [Header("Items")]
    [SerializeField] private Item _textbookSpore;
    [SerializeField] private Item _blackboardSpore;
    [SerializeField] private Item _mealSpore;

    [Header("Sound")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _successClip;

    [Header("Combination")]
    [SerializeField] private List<CombinationRecipe> _combinationRecipes = new List<CombinationRecipe>();

    private Dictionary<ItemPair, Item> _combinationDictionary;

    [System.Serializable]
    private struct CombinationRecipe
    {
        public Item ItemA;
        public Item ItemB;
        public Item Result;
    }

    private struct ItemPair : System.IEquatable<ItemPair>
    {
        public Item ItemA;
        public Item ItemB;

        public ItemPair(Item itemA, Item itemB)
        {
            if (CompareItems(itemA, itemB) <= 0)
            {
                ItemA = itemA;
                ItemB = itemB;
            }
            else
            {
                ItemA = itemB;
                ItemB = itemA;
            }
        }

        public bool Equals(ItemPair other)
        {
            return AreSameItem(ItemA, other.ItemA) && AreSameItem(ItemB, other.ItemB);
        }

        public override bool Equals(object obj)
        {
            return obj is ItemPair other && Equals(other);
        }

        public override int GetHashCode()
        {
            int hashA = ItemA != null ? (ItemA.ItemName.GetHashCode() * 397) ^ ItemA.ShopOrder : 0;
            int hashB = ItemB != null ? (ItemB.ItemName.GetHashCode() * 397) ^ ItemB.ShopOrder : 0;
            return hashA ^ (hashB << 1);
        }

        private static int CompareItems(Item a, Item b)
        {
            if (ReferenceEquals(a, b))
            {
                return 0;
            }

            if (a == null)
            {
                return -1;
            }

            if (b == null)
            {
                return 1;
            }

            int nameComparison = string.Compare(a.ItemName, b.ItemName, System.StringComparison.Ordinal);
            if (nameComparison != 0)
            {
                return nameComparison;
            }

            return a.ShopOrder.CompareTo(b.ShopOrder);
        }

        private static bool AreSameItem(Item a, Item b)
        {
            if (a == null || b == null)
            {
                return false;
            }

            return a == b || (a.ItemName == b.ItemName && a.ShopOrder == b.ShopOrder);
        }
    }

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
        if (Instance != null && Instance != this)
        {
            Destroy(transform.root.gameObject); // 중복 생성 방지
            return;
        }

        Instance = this;
        DontDestroyOnLoad(transform.root.gameObject); // 씬을 이동해도 인벤토리 캔버스 유지

        InitializeCombinationPairs();

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
        Item loadedPotItem = Resources.Load<Item>("Items/PotItems/NormalPotItem");
        PutItem(loadedPotItem);

        // 버섯 수확 이벤트 구독
        Pot.OnMushroomReaped += HandleMushroomReaped;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }

        // 버섯 수확 이벤트 구독 해제
        Pot.OnMushroomReaped -= HandleMushroomReaped;
    }

    // 버섯 수확 시 인벤토리에 추가
    private void HandleMushroomReaped(Item mushroomItem)
    {
        Debug.Log($"InventoryManager.HandleMushroomReaped 호출: mushroomItem={mushroomItem?.ItemName}");
        
        if (mushroomItem != null)
        {
            PutItem(mushroomItem);
        }
        else
        {
            Debug.LogError("HandleMushroomReaped: mushroomItem이 NULL입니다!");
        }
    }

    private void InitializeCombinationPairs()
    {
        _combinationDictionary = new Dictionary<ItemPair, Item>();

        foreach (CombinationRecipe recipe in _combinationRecipes)
        {
            if (recipe.ItemA == null || recipe.ItemB == null || recipe.Result == null)
            {
                continue;
            }

            ItemPair pair = new ItemPair(recipe.ItemA, recipe.ItemB);
            if (!_combinationDictionary.ContainsKey(pair))
            {
                _combinationDictionary.Add(pair, recipe.Result);
            }
        }

        if (_textbookSpore != null && _blackboardSpore != null && _mealSpore != null)
        {
            ItemPair textbookBlackboardPair = new ItemPair(_textbookSpore, _blackboardSpore);
            if (!_combinationDictionary.ContainsKey(textbookBlackboardPair))
            {
                _combinationDictionary.Add(textbookBlackboardPair, _mealSpore);
            }
        }
    }

    // 인벤토리에 아이템 추가: 같은 아이템 찾으면 개수 증가, 없으면 새 슬롯에 추가
    public void PutItem(Item item)
    {
        foreach (InventorySlot slot in _slots)
        {
            if (slot.Item != null && AreSameItem(slot.Item, item))
            {
                slot.AddCount(1);
                return;
            }
        }

        foreach (InventorySlot slot in _slots)
        {
            if (slot.Item == null)
            {
                slot.SetItem(item, 1);
                RefreshAllSlots();
                return;
            }
        }
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
        if (draggedItem == null || targetItem == null || _combinationDictionary == null)
        {
            return;
        }

        ItemPair pair = new ItemPair(draggedItem, targetItem);
        if (!_combinationDictionary.TryGetValue(pair, out Item resultItem))
        {
            return;
        }

        int draggedCount = GetItemCount(draggedItem);
        int targetCount = GetItemCount(targetItem);

        if (AreSameItem(draggedItem, targetItem))
        {
            if (draggedCount < 2)
            {
                return;
            }
        }
        else
        {
            if (draggedCount < 1)
            {
                return;
            }

            if (targetCount < 1)
            {
                return;
            }
        }

        RemoveItem(draggedItem, 1);
        RemoveItem(targetItem, 1);
        PutItem(resultItem);

        if (_audioSource != null && _successClip != null)
        {
            _audioSource.PlayOneShot(_successClip);
        }

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
    }
}