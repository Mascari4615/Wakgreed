using UnityEngine;
using System.Collections.Generic;

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
    protected List<GameObject> DoorHiders { get; private set; } = new();
    protected List<GameObject> DoorParticles { get; private set; } = new();

    public void Initialize(Vector2 _coordinate, bool[] _isConnectToNearbyRoom)
    {
        Coordinate = _coordinate;
        IsConnectToNearbyRoom = _isConnectToNearbyRoom;
        transform.localPosition = Coordinate * 100;

        Doors[0] = transform.Find("Door_Up").gameObject;
        Doors[1] = transform.Find("Door_Down").gameObject;
        Doors[2] = transform.Find("Door_Left").gameObject;
        Doors[3] = transform.Find("Door_Right").gameObject;

        for (int i = 0; i < 4; i++)
        {
            if (IsConnectToNearbyRoom[i])
            {
                DoorParticles.Add(Doors[i].transform.Find("Particle").gameObject);
                Doors[i].transform.Find("Particle").gameObject.SetActive(true);
                DoorHiders.Add(Doors[i].transform.Find("Hider").gameObject);
                Doors[i].transform.Find("Wall").gameObject.SetActive(false);
                Doors[i].transform.Find("Debug").gameObject.SetActive(true);
            }   
        }
    }

    public abstract void Enter();
}
