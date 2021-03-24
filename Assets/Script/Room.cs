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
    public Vector2 coordinate { get; private set; }
    public bool isVisited { get; protected set; } = false;
    public bool[] isConnectToNearbyRoom { get; private set; } = { false, false, false, false };
    public GameObject[] doors { get; private set; } = new GameObject[4];
    protected GameObject[] doorHiders { get; private set; } = new GameObject[4];
    
    public void Initialize(Vector2 _coordinate, bool[] _isConnectToNearbyRoom)
    {
        coordinate = _coordinate;
        isConnectToNearbyRoom = _isConnectToNearbyRoom;
        transform.localPosition = coordinate * 100;

        doors[0] = transform.Find("Door_Up").gameObject;
        doors[1] = transform.Find("Door_Down").gameObject;
        doors[2] = transform.Find("Door_Left").gameObject;
        doors[3] = transform.Find("Door_Right").gameObject;

        doorHiders[0] = transform.Find("Hider_Up").gameObject;
        doorHiders[1] = transform.Find("Hider_Down").gameObject;
        doorHiders[2] = transform.Find("Hider_Left").gameObject;
        doorHiders[3] = transform.Find("Hider_Right").gameObject;

        doors[0].SetActive(isConnectToNearbyRoom[0]);
        doors[1].SetActive(isConnectToNearbyRoom[1]);
        doors[2].SetActive(isConnectToNearbyRoom[2]);
        doors[3].SetActive(isConnectToNearbyRoom[3]);
    }

    public abstract void Enter();
}
