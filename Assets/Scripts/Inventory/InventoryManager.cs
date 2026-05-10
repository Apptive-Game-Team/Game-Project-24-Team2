using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance { get; private set; }

    [Header("Pencil Texts")]
    [SerializeField] private TMP_Text _pencilMushroomText;
    [SerializeField] private TMP_Text _pencilSporeText;

    [Header("Textbook Texts")]
    [SerializeField] private TMP_Text _textbookMushroomText;
    [SerializeField] private TMP_Text _textbookSporeText;

    [Header("Blackboard Texts")]
    [SerializeField] private TMP_Text _blackboardMushroomText;
    [SerializeField] private TMP_Text _blackboardSporeText;

    [Header("Meal Texts")]
    [SerializeField] private TMP_Text _mealMushroomText;
    [SerializeField] private TMP_Text _mealSporeText;

    [Header("Sound")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _successClip;

    private Dictionary<string, int> _counts = new Dictionary<string, int>();

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SetCount(ItemType.Pencil, ItemProperty.Mushroom, 0);
        SetCount(ItemType.Pencil, ItemProperty.Spore, 0);

        SetCount(ItemType.Textbook, ItemProperty.Mushroom, 3);
        SetCount(ItemType.Textbook, ItemProperty.Spore, 0);

        SetCount(ItemType.Blackboard, ItemProperty.Mushroom, 3);
        SetCount(ItemType.Blackboard, ItemProperty.Spore, 0);

        SetCount(ItemType.Meal, ItemProperty.Mushroom, 0);
        SetCount(ItemType.Meal, ItemProperty.Spore, 0);

        UpdateUI();
    }

    public void TryCombine(ItemType draggedType, ItemType targetType)
    {
        bool isTextbookAndBlackboard =
            (draggedType == ItemType.Textbook && targetType == ItemType.Blackboard) ||
            (draggedType == ItemType.Blackboard && targetType == ItemType.Textbook);

        if (!isTextbookAndBlackboard)
        {
            Debug.Log("조합 실패");
            return;
        }

        if (GetCount(ItemType.Textbook, ItemProperty.Mushroom) < 1)
        {
            Debug.Log("교과서 버섯 부족");
            return;
        }

        if (GetCount(ItemType.Blackboard, ItemProperty.Mushroom) < 1)
        {
            Debug.Log("칠판 버섯 부족");
            return;
        }

        AddCount(ItemType.Textbook, ItemProperty.Mushroom, -1);
        AddCount(ItemType.Blackboard, ItemProperty.Mushroom, -1);
        AddCount(ItemType.Meal, ItemProperty.Spore, 1);

        UpdateUI();

        if (_audioSource != null && _successClip != null)
        {
            _audioSource.PlayOneShot(_successClip);
        }

        Debug.Log("조합 성공: 교과서 버섯 + 칠판 버섯 = 급식 포자");
    }

    private string GetKey(ItemType type, ItemProperty property)
    {
        return type.ToString() + "_" + property.ToString();
    }

    private int GetCount(ItemType type, ItemProperty property)
    {
        string key = GetKey(type, property);

        if (!_counts.ContainsKey(key))
        {
            _counts[key] = 0;
        }

        return _counts[key];
    }

    private void SetCount(ItemType type, ItemProperty property, int count)
    {
        _counts[GetKey(type, property)] = count;
    }

    private void AddCount(ItemType type, ItemProperty property, int amount)
    {
        _counts[GetKey(type, property)] = GetCount(type, property) + amount;
    }

    private void UpdateUI()
    {
        _pencilMushroomText.text =
            "버섯 x" + GetCount(ItemType.Pencil, ItemProperty.Mushroom);

        _pencilSporeText.text =
            "포자 x" + GetCount(ItemType.Pencil, ItemProperty.Spore);

        _textbookMushroomText.text =
            "버섯 x" + GetCount(ItemType.Textbook, ItemProperty.Mushroom);

        _textbookSporeText.text =
            "포자 x" + GetCount(ItemType.Textbook, ItemProperty.Spore);

        _blackboardMushroomText.text =
            "버섯 x" + GetCount(ItemType.Blackboard, ItemProperty.Mushroom);

        _blackboardSporeText.text =
            "포자 x" + GetCount(ItemType.Blackboard, ItemProperty.Spore);

        _mealMushroomText.text =
            "버섯 x" + GetCount(ItemType.Meal, ItemProperty.Mushroom);

        _mealSporeText.text =
            "포자 x" + GetCount(ItemType.Meal, ItemProperty.Spore);
    }
}