using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [Header("조합에 사용될 아이템")]
    [SerializeField] private Item _textbookMushroom;
    [SerializeField] private Item _blackboardMushroom;
    [SerializeField] private Item _mealSpore;

    [Header("사운드")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _successClip;

    [Header("이펙트")]
    [SerializeField] private GameObject _combineEffectPrefab;
    [SerializeField] private Transform _effectSpawnPoint;

    // 아이템 개수 저장
    private Dictionary<Item, int> _counts =
        new Dictionary<Item, int>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // 시작 아이템 설정
        SetCount(_textbookMushroom, 3);
        SetCount(_blackboardMushroom, 3);
        SetCount(_mealSpore, 0);
    }

    // 아이템 개수 반환
    private int GetCount(Item item)
    {
        if (!_counts.ContainsKey(item))
        {
            _counts[item] = 0;
        }

        return _counts[item];
    }

    // 아이템 개수 설정
    private void SetCount(Item item, int count)
    {
        _counts[item] = count;
    }

    // 아이템 개수 추가/감소
    private void AddCount(Item item, int amount)
    {
        _counts[item] = GetCount(item) + amount;
    }

    // 인벤토리에 아이템 추가
    public void PutItem(Item item)
    {
        AddCount(item, 1);

        Debug.Log(item.ItemName + " 획득");
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
        if (GetCount(_textbookMushroom) < 1)
        {
            Debug.Log("교과서 버섯 부족");
            return;
        }

        if (GetCount(_blackboardMushroom) < 1)
        {
            Debug.Log("칠판 버섯 부족");
            return;
        }

        // 재료 제거
        AddCount(_textbookMushroom, -1);
        AddCount(_blackboardMushroom, -1);

        // 결과 아이템 추가
        AddCount(_mealSpore, 1);

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