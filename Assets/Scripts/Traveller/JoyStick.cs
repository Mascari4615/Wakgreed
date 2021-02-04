using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class JoyStick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    public bool isDynamic;
    public bool isStatic;

    [SerializeField] RectTransform rect_Background = null;
    [SerializeField] public RectTransform rect_Joystick = null;
    private float radius = 0;

    public Vector2 inputValue = Vector2.zero;
    public bool isDraging = false;

    void Awake()
    {
        radius = rect_Background.rect.width * 0.5f * 0.9f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 direction = (Vector3)eventData.position - rect_Background.position;

        if (Vector3.Distance((Vector3)eventData.position, rect_Background.position) > radius)
            rect_Background.position = (Vector3)eventData.position - direction.normalized * radius;

        rect_Joystick.position = (Vector3)eventData.position;
        inputValue = direction.normalized;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isDraging = true;
        rect_Background.position = (Vector3)eventData.position;
    }

    public void OnPointerUp(PointerEventData eventData)
    {        
        isDraging = false;
        rect_Joystick.localPosition = Vector3.zero;
    }
}
