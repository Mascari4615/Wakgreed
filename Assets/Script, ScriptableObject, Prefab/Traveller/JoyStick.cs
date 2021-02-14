using UnityEngine;
using UnityEngine.EventSystems;

public class JoyStick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    [SerializeField] private RectTransform rect_Background = null;
    [SerializeField] private RectTransform rect_Joystick = null;
    public Vector2 inputValue;
    public bool isInputting;

    public void OnDrag(PointerEventData eventData)
    {
        rect_Joystick.position = (Vector3)eventData.position;
        Vector3 direction = rect_Joystick.position - rect_Background.position;

        if (Vector3.Distance(rect_Joystick.position, rect_Background.position) > rect_Background.rect.width / 2)
            rect_Background.position = rect_Joystick.position - direction.normalized * rect_Background.rect.width / 2;

        inputValue = direction.normalized;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isInputting = true;
        rect_Background.position = (Vector3)eventData.position;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isInputting = false;
        rect_Joystick.localPosition = Vector3.zero;
    }
}
