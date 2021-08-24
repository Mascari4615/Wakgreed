using UnityEngine;

public class MouseManager : MonoBehaviour
{
    public Texture2D cursorTextureA;
    public Texture2D cursorTextureB;
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;
    
    void Start() { }

    private void Awake()
    {
        Cursor.SetCursor(cursorTextureA, hotSpot, cursorMode);
        //hotSpot.x = cursorTextureA.width / 2;
        //hotSpot.y = cursorTextureA.height / 2;

        Debug.Log("Mouse");
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