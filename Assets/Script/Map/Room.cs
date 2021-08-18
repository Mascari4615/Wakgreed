using UnityEngine;

public enum RoomType
{
    Spawn,
    Portal,
    Boss,
    Shop,
    Interaction,
    Normal,
}

public abstract class Room : MonoBehaviour
{
    public RoomType roomType = RoomType.Normal;
    public Vector2 Coordinate { get; private set; }
    public bool IsVisited { get; protected set; } = false;
    public bool[] IsConnectToNearbyRoom { get; private set; } = { false, false, false, false };
    public GameObject[] Doors { get; private set; } = new GameObject[4];
    protected GameObject[] DoorHiders { get; private set; } = new GameObject[4];

    public void Initialize(Vector2 _coordinate, bool[] _isConnectToNearbyRoom)
    {
        Coordinate = _coordinate;
        IsConnectToNearbyRoom = _isConnectToNearbyRoom;
        transform.localPosition = Coordinate * 100;

        Doors[0] = transform.Find("Door_Up").gameObject;
        Doors[1] = transform.Find("Door_Down").gameObject;
        Doors[2] = transform.Find("Door_Left").gameObject;
        Doors[3] = transform.Find("Door_Right").gameObject;

        DoorHiders[0] = transform.Find("Hider_Up").gameObject;
        DoorHiders[1] = transform.Find("Hider_Down").gameObject;
        DoorHiders[2] = transform.Find("Hider_Left").gameObject;
        DoorHiders[3] = transform.Find("Hider_Right").gameObject;

        Doors[0].SetActive(IsConnectToNearbyRoom[0]);
        Doors[1].SetActive(IsConnectToNearbyRoom[1]);
        Doors[2].SetActive(IsConnectToNearbyRoom[2]);
        Doors[3].SetActive(IsConnectToNearbyRoom[3]);
    }

    public abstract void Enter();
}
