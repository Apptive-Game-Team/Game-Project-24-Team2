using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [Header("Pencil Texts")]
    public TMP_Text pencilMushroomText;
    public TMP_Text pencilSporeText;

    [Header("Textbook Texts")]
    public TMP_Text textbookMushroomText;
    public TMP_Text textbookSporeText;

    [Header("Blackboard Texts")]
    public TMP_Text blackboardMushroomText;
    public TMP_Text blackboardSporeText;

    [Header("Meal Texts")]
    public TMP_Text mealMushroomText;
    public TMP_Text mealSporeText;

    [Header("Sound")]
    public AudioSource audioSource;
    public AudioClip successClip;

    private Dictionary<string, int> counts = new Dictionary<string, int>();

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

        if (audioSource != null && successClip != null)
        {
            audioSource.PlayOneShot(successClip);
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

        if (!counts.ContainsKey(key))
        {
            counts[key] = 0;
        }

        return counts[key];
    }

    private void SetCount(ItemType type, ItemProperty property, int count)
    {
        counts[GetKey(type, property)] = count;
    }

    private void AddCount(ItemType type, ItemProperty property, int amount)
    {
        counts[GetKey(type, property)] = GetCount(type, property) + amount;
    }

    private void UpdateUI()
    {
        pencilMushroomText.text = "버섯 x" + GetCount(ItemType.Pencil, ItemProperty.Mushroom);
        pencilSporeText.text = "포자 x" + GetCount(ItemType.Pencil, ItemProperty.Spore);

        textbookMushroomText.text = "버섯 x" + GetCount(ItemType.Textbook, ItemProperty.Mushroom);
        textbookSporeText.text = "포자 x" + GetCount(ItemType.Textbook, ItemProperty.Spore);

        blackboardMushroomText.text = "버섯 x" + GetCount(ItemType.Blackboard, ItemProperty.Mushroom);
        blackboardSporeText.text = "포자 x" + GetCount(ItemType.Blackboard, ItemProperty.Spore);

        mealMushroomText.text = "버섯 x" + GetCount(ItemType.Meal, ItemProperty.Mushroom);
        mealSporeText.text = "포자 x" + GetCount(ItemType.Meal, ItemProperty.Spore);
    }
}