using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System;
using Unity.VisualScripting;
using UnityEngine.Rendering;
using System.IO;

public class Drag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler 
{   
    public static event Action OnPotPlaced;
    public static event Action OnSporePlaced;
    [SerializeField] private UnityEngine.UI.Image image;
    [SerializeField] private Item _dragItemSO;
    [SerializeField] private GameObject[] Pots; 
    private RectTransform _rectTransform;

    private void Start()
    {
        VisibleGuide(false);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        _rectTransform = GetComponent<RectTransform>();
        transform.SetParent(transform.root);    //Grid에서 벗어나도록
        image.raycastTarget = false;    //raycast가 화분 감지x, PotLocation감지o

        VisibleGuide(true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Mouse.current.position.ReadValue();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        OrderUI(_dragItemSO);

        Place(eventData.position); //화분이든 포자든 심어드립니다
        VisibleGuide(false);
        image.raycastTarget = true;
    }

    private void OrderUI(Item draggingItemSO) //화분생성, 연필포자 등의 위치조절
    {
        switch (draggingItemSO)
        {
            case PotItem:
                _rectTransform.anchoredPosition = new Vector2(350f, 175f);
                break;
            case PencilSporeItem:
                _rectTransform.anchoredPosition = new Vector2(350f, 65f);
                break;
            case TextbookSporeItem:
                _rectTransform.anchoredPosition = new Vector2(350f, -5f);
                break;
            case BlackboardSporeItem:
                _rectTransform.anchoredPosition = new Vector2(350f, -75f);
                break;
            case MealSporeItem:
                _rectTransform.anchoredPosition = new Vector2(350f, -145f);
                break;
            default:
                break;
        }
    }

    private void VisibleGuide(bool isVisible) //배치 가능 위치 표시/안표시
    {
        for(int i = 0; i < Pots.Length; i++)
        {
            Pot pot = Pots[i].GetComponent<Pot>();
            if (!pot.IsPotPlaced)
            {
                if (isVisible)
                {
                    pot.ShowGuide();
                }
                else
                {
                    pot.HideGuide();
                }
            }
        }
    }

    private void Place(Vector2 screenPos)
    {
        Vector2 worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        Collider2D hitCollider = Physics2D.OverlapPoint(worldPos);
        if (hitCollider != null)
        {
            GameObject hitObject = hitCollider.gameObject;
            Pot pot = hitObject.GetComponent<Pot>();
            if(_dragItemSO is PotItem)
                {
                    pot.ShowPot();
                    pot.IsPotPlaced = true;
                    OnPotPlaced?.Invoke();
                }
            else
                {
                    if (pot.IsPotPlaced)
                    {
                        pot.ShowSpore(_dragItemSO);
                        pot.IsSporePlaced = true;
                        OnSporePlaced?.Invoke();
                    }    
                }
        }
    }
}
