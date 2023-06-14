using UnityEngine;

public class RoomSlot : MonoBehaviour
{
    [HideInInspector] public Vector2 coordinate;

    public void Teleport()
    {
        GameManager.Instance.StartCoroutine(StageManager.Instance.MigrateRoom(coordinate));
    }
}