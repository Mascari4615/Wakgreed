using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageManager : MonoBehaviour
{    
    private class RoomStack
    {
        public int originalX, originalY, totalX, totalY = 0;
        public string direction = "";

        public RoomStack(int x, int y, string _direction) // 선언과 동시에 실행되는 함수
        {
            originalX = x;
            originalY = y;
            totalX = x;
            totalY = y;
            direction = _direction;
            
            if (direction == "Up") totalY = y + 1;
            else if (direction == "Down") totalY = y - 1;
            else if (direction == "Left") totalX = x - 1;
            else if (direction == "Right")totalX = x + 1;
        }
    }

    private static StageManager instance = null;
    [HideInInspector] public static StageManager Instace { get { return instance; } }

    [SerializeField] private GameObject stageGrid;
    private int roomCount = 0;
    // private Queue<RoomStack> roomStackQueue = new Queue<RoomStack>();
    private Stack<RoomStack> roomStackStack = new Stack<RoomStack>();
    private List<Room> roomDatas = new List<Room>();
    private List<Room> rooms = new List<Room>();
    private Dictionary<Vector2, Room> roomDictionary = new Dictionary<Vector2, Room>();
    private int roomMoldLength;

    void Awake()
    {
        instance = this;
    }

    public void GenerateStage(int currentStageID, int _roomMoldLength, int maxRoomCount) // 스테이지 만들기 시작
    {
        Debug.Log("----");
        roomMoldLength = _roomMoldLength;

        DestroyStage();

        rooms.Clear();
        roomCount = maxRoomCount;
        // roomStackQueue.Clear();
        roomStackStack.Clear();
        roomDatas.Clear();
        roomDatas = new List<Room>(StageDataBase.Instance.stages[currentStageID].roomDatas); 
        roomDictionary.Clear();

        // 가운데에 가장 기본 방 생
        GenerateRoom(0, (roomMoldLength - 1) / 2, (roomMoldLength - 1) / 2);
        roomCount--;
        
        while (roomCount > 0)
        {
            // Debug.Log($"Queue {roomStackQueue.Count}");
            Debug.Log($"List {rooms.Count}");

            // if (roomStackQueue.Count == 0)
            if (roomStackStack.Count == 0)
            {
                Room room = rooms[Random.Range(0, rooms.Count)];
                GenerateRoomStack(room.x, room.y);
                continue;
            }
            
            RoomStack stack = roomStackStack.Pop();

            Room totalRoom;
            if (roomDictionary.TryGetValue(new Vector2(stack.totalX, stack.totalY), out totalRoom)) continue;
            Debug.Log($"RoomStackTotal {stack.originalX} {stack.originalY} > {stack.totalX} {stack.totalY}");

            GenerateRoom(Random.Range(0, roomDatas.Count), stack.totalX, stack.totalY);

            int origin = 0;
            int total = 0;

            if (stack.direction == "Up") {origin = 0; total = 1;}
            else if (stack.direction == "Down") {origin = 1; total = 0;}
            else if (stack.direction == "Left") {origin = 2; total = 3;}
            else if (stack.direction == "Right") {origin = 3; total = 2;}
    
            roomDictionary[new Vector2(stack.originalX, stack.originalY)].isDoorOpen[origin] = true;
            roomDictionary[new Vector2(stack.totalX, stack.totalY)].isDoorOpen[total] = true;

            roomCount--;
        }

        StartCoroutine(GameManager.Instance.StartStage(new Vector2((roomMoldLength - 1) / 2, (roomMoldLength - 1) / 2), roomDictionary));
    }

    private void GenerateRoom(int roomDataIndex, int x, int y)
    {
        Debug.Log($"GenerateRoom {x} {y}");

        GameObject go = Instantiate(roomDatas[roomDataIndex].gameObject, stageGrid.transform);
        go.transform.localPosition = new Vector3(x, y, 0) * 100;
        Room rd = go.GetComponent<Room>();
        rd.SetRoomCoordinate(x, y);
        rooms.Add(rd);

        GenerateRoomStack(x, y);

        roomDictionary.Add(new Vector2(x, y), rd);
        roomDatas.RemoveAt(roomDataIndex);
    }
    
    public void DestroyStage()
    {
        for (int i = 0; i < stageGrid.transform.childCount; i++)
        {
            Destroy(stageGrid.transform.GetChild(i).gameObject);
        }
    }

    private void GenerateRoomStack(int x, int y)
    {
        if (Random.Range(0, 3) == 0 && y != roomMoldLength - 1) roomStackStack.Push(new RoomStack(x, y, "Up"));
        if (Random.Range(0, 3) == 0 && y != 0) roomStackStack.Push(new RoomStack(x, y, "Down"));
        if (Random.Range(0, 3) == 0 && x != 0) roomStackStack.Push(new RoomStack(x, y, "Left"));
        if (Random.Range(0, 3) == 0 && x != roomMoldLength - 1) roomStackStack.Push(new RoomStack(x, y, "Right"));
    }
}

