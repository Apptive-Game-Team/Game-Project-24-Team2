using Microsoft.Unity.VisualStudio.Editor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using System;

public class Drag : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerUpHandler, IPointerDownHandler 
{
    public static event Action<bool> OnDragging;
    
    //[HideInInspector] public Transform parentAfterDrag;
    public UnityEngine.UI.Image image;
    public int type;  //0:화분  1:포자
    private RectTransform rectTransform;
    public void OnBeginDrag(PointerEventData eventData)
    {

        rectTransform = GetComponent<RectTransform>();
        transform.SetParent(transform.root);    //Grid에서 벗어나도록
        image.raycastTarget = false;    //raycast가 화분 감지x, PotLocation감지o
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = Mouse.current.position.ReadValue();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition = new Vector2(-100f, -100f);
        image.raycastTarget = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        OnDragging.Invoke(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDragging.Invoke(true);
    }
}
