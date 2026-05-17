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
    
    [Header("슬롯")]
    [SerializeField] private InventorySlot[] _slots;


    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        PutItem(_textbookMushroom);
        PutItem(_textbookMushroom);
        PutItem(_blackboardMushroom);
    }


    // 인벤토리에 아이템 추가
    public void PutItem(Item item)
    {
        // 이미 아이템이 있는 슬롯 찾기
        foreach (InventorySlot slot in _slots)
        {
            if (slot.Item == item)
            {
                slot.AddCount(1);

                Debug.Log(item.ItemName + " 개수 증가");

                return;
            }
        }

        // 빈 슬롯 찾기
        foreach (InventorySlot slot in _slots)
        {
            if (slot.Item == null)
            {
                slot.SetItem(item, 1);

                Debug.Log(item.ItemName + " 새 슬롯 추가");

                return;
            }
        }

        Debug.Log("인벤토리가 가득 찼습니다.");
    }

    // 특정 아이템 개수 반환
    private int GetItemCount(Item item)
    {
        foreach (InventorySlot slot in _slots)
        {
            if (slot.Item == item)
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
            if (slot.Item == item)
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