using UnityEngine;
using UnityEngine.EventSystems;

public class MouseManager : MonoBehaviour
{
    [SerializeField] private Texture2D cursorTextureA;
    [SerializeField] private Texture2D cursorTextureB;
    [SerializeField] private CursorMode cursorMode = CursorMode.Auto;
    private MouseType mouseType;

    private void Awake()
    {
        ChangeMouseAMode();
    }

    private void Update()
    {
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (mouseType == MouseType.Arrow)
            {
                ChangeMouseBMode();
            }
        }
        else
        {
            if (mouseType == MouseType.Attack)
            {
                ChangeMouseAMode();
            }
        }
    }

    public void ChangeMouseAMode()
    {
        mouseType = MouseType.Arrow;
        Cursor.SetCursor(cursorTextureA, Vector2.zero, cursorMode);
    }

    public void ChangeMouseBMode()
    {
        mouseType = MouseType.Attack;
        // Cursor.SetCursor(cursorTextureB, new Vector2(cursorTextureB.width / 2, cursorTextureB.height / 2), cursorMode);
        Cursor.SetCursor(cursorTextureB, Vector2.zero, cursorMode);
    }

    private enum MouseType
    {
        Arrow,
        Attack
    }
}