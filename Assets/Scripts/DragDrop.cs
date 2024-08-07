using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragDrop : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{

    private RectTransform rect;
    [SerializeField] Canvas canvas;
    private void Awake()
    {
        rect = GetComponent<RectTransform>();

    }

    public void OnBeginDrag(PointerEventData eventData)
    {

    }
    public void OnDrag(PointerEventData eventData)
    {
        rect.anchoredPosition += eventData.delta / canvas.scaleFactor;
        print("CLICKED = ");

    }
    public void OnEndDrag(PointerEventData eventData)
    {

    }

}
