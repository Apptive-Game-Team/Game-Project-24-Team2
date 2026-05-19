using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class PotLocation : MonoBehaviour, IDropHandler
{
    public static event Action OnPotPlaced;
    public static event Action OnSporePlaced;
    [SerializeField] private int LocationIndex;     //어느 위치에 있는 화분or포자인지 확인
    [SerializeField] private GameObject[] Pots; 
    [SerializeField] private GameObject[] Spores;
    [SerializeField] private GameObject[] Mushrooms;
    private UnityEngine.UI.Image _potLocationImage;
    private void Start()
    {
        _potLocationImage = GetComponent<UnityEngine.UI.Image>();
        _potLocationImage.enabled = false;
        for (int i = 0; i < Pots.Length; i++)
        {
            Pots[i].SetActive(false);
            Spores[i].SetActive(false);
            Mushrooms[i].SetActive(false);
        }
    }
    public void OnDrop(PointerEventData eventData) //화분 드롭 시 위치 잡기
    {
        GameObject draggingPot = eventData.pointerDrag;
        Drag dragItem = draggingPot.GetComponent<Drag>();
        _potLocationImage.enabled = false;
        if (dragItem.type == 0)
        {
            Pots[LocationIndex].SetActive(true);
            OnPotPlaced?.Invoke();
        }
        else if (dragItem.type == 1)
        {
            Spores[LocationIndex].SetActive(true);
            Pot changeIsPotted = Pots[LocationIndex].GetComponent<Pot>();
            changeIsPotted.IsPotted = true;
            OnSporePlaced?.Invoke();
        }
    }

    private void OnEnable()
    {
        Drag.OnDragging += ActivateLocationImage;
        Pot.OnGrowingDone += MakeMushroom;
        Pot.OnMushroomReaped += UnseenMushroom;
    }

    private void OnDisable()
    {
        Drag.OnDragging -= ActivateLocationImage;
    }

    private void ActivateLocationImage(bool isDragging) //화분 배치 가능한 위치를 반투명하게 표시
    {
        if (_potLocationImage != null)
        {
            _potLocationImage.enabled = isDragging;
        }
    }

    private void MakeMushroom(int potIndex) //포자 -> 버섯으로 자라는 것처럼 표시
    {
        Spores[potIndex].SetActive(false);
        Mushrooms[potIndex].SetActive(true);
    }

    private void UnseenMushroom(int potIndex)
    {
        Mushrooms[potIndex].SetActive(false);
    }
}
