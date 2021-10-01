using UnityEngine;

public class MouseManager : MonoBehaviour
{
    [SerializeField] private Texture2D cursorTextureA;
    [SerializeField] private Texture2D cursorTextureB;
    [SerializeField] private CursorMode cursorMode = CursorMode.Auto;
    [SerializeField] private Vector2 hotSpot = Vector2.zero;

    private void Awake()
    {
        Cursor.SetCursor(cursorTextureA, hotSpot, cursorMode);
        //hotSpot.x = cursorTextureA.width / 2;
        //hotSpot.y = cursorTextureA.height / 2;
    }

    public void ChangeMouseAMode()
    {
        Cursor.SetCursor(cursorTextureA, hotSpot, cursorMode);
    }
    public void ChangeMouseBMode()
    {
        Cursor.SetCursor(cursorTextureB, hotSpot, cursorMode);
    }
}