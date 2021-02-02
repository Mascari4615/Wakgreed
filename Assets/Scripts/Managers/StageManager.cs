using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomMold
{
    public RoomData roomData = new RoomData();

    public int x, y;
    public bool[] isDoorOpen = new bool[4];

    public RoomMold(int _x, int _y) // 선언과 동시에 실행되는 함수
    {
        x = _x;
        y = _y;

        GenerateRoomStack();
    }

    public void GenerateRoomStack()
    {
        if (Random.Range(0, 2) == 0 && y != StageManager.Instace.roomLength - 1) StageManager.Instace.roomStackQueue.Enqueue(new RoomStack(x, y, "Up"));
        if (Random.Range(0, 2) == 0 && y != 0) StageManager.Instace.roomStackQueue.Enqueue(new RoomStack(x, y, "Down"));
        if (Random.Range(0, 2) == 0 && x != 0) StageManager.Instace.roomStackQueue.Enqueue(new RoomStack(x, y, "Left"));
        if (Random.Range(0, 2) == 0 && x != StageManager.Instace.roomLength - 1) StageManager.Instace.roomStackQueue.Enqueue(new RoomStack(x, y, "Right"));
    }
}

public class RoomStack
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

public class StageManager : MonoBehaviour
{    
    private static StageManager instance = null;
    [HideInInspector] public static StageManager Instace { get { return instance; } }

    public int maxRoomCount = 0;
    public int roomLength = 0;

    [SerializeField] private GameObject stageGrid;
    private RoomMold[,] rooms = new RoomMold[0, 0];
    private int roomCount = 0;
    [HideInInspector] public Queue<RoomStack> roomStackQueue = new Queue<RoomStack>();
    private List<RoomData> roomDataList = new List<RoomData>();
    private List<RoomMold> roomList = new List<RoomMold>();
    private Dictionary<Vector2, RoomMold> roomDictionary = new Dictionary<Vector2, RoomMold>();

    void Awake()
    {
        instance = this;
    }

    void Initialize()
    {
        DestroyStage();

        rooms.Initialize();
        rooms = new RoomMold[roomLength, roomLength];
        roomCount = 0;
        roomStackQueue.Clear();
        roomDataList.Clear();
        roomList.Clear();
        roomDictionary.Clear();
    }

    public void GenerateStage(int currentStageID) // 스테이지 만들기 시작
    {
        Initialize();

        // 방 데이터 리스트에, 현재 스테이지의 방 데이터 배열을 대입
        roomDataList = new List<RoomData>(StageDataBase.Instance.stages[currentStageID].roomDatas); 

        // 가운데에 가장 기본 방 생성, 방 리스트에 넣기
        rooms[(roomLength - 1) / 2, (roomLength - 1) / 2] = new RoomMold((roomLength - 1) / 2, (roomLength - 1) / 2);
        roomList.Add(rooms[(roomLength - 1) / 2, (roomLength - 1) / 2]);
        roomCount = maxRoomCount - 1;
        
        GenerateRoom();
    }

    private void GenerateRoom()
    {
        while (roomCount > 0)
        {
            if (roomStackQueue.Count == 0)
            {
                roomList[Random.Range(0, roomList.Count)].GenerateRoomStack();
                continue;
            }
            
            RoomStack stack = roomStackQueue.Dequeue();

            if (rooms[stack.totalX, stack.totalY] != null) continue;
            
            rooms[stack.totalX, stack.totalY] = new RoomMold(stack.totalX, stack.totalY);
            int origin = 0;
            int total = 0;;

            if (stack.direction == "Up") {origin = 0; total = 1;}
            else if (stack.direction == "Down") {origin = 1; total = 0;}
            else if (stack.direction == "Left") {origin = 2; total = 3;}
            else if (stack.direction == "Right") {origin = 3; total = 2;}
    
            rooms[stack.originalX, stack.originalY].isDoorOpen[origin] = true;
            rooms[stack.totalX, stack.totalY].isDoorOpen[total] = true;

            roomList.Add(rooms[stack.totalX, stack.totalY]);
            roomCount--;
        }

        PrintRoomMolds();

        int roomListCount = roomList.Count;
        Vector2 spawnRoomCoordinate = Vector2.zero;

        for (int i = 0; i < roomListCount; i++) // 랜덤한 방 그릇에 랜덤한 방 데이터 대입
        {      
            int roomListIndex = Random.Range(0, roomList.Count); // 방 구조용
            int roomDataIndex = Random.Range(0, roomDataList.Count); // 방 데이터용

            if (i == 0) // 0 --- 스폰
            {
                roomListIndex = 0;
                roomDataIndex = 0;;
                
                spawnRoomCoordinate = new Vector2(roomList[roomListIndex].x, roomList[roomListIndex].y);
            }
            else if (i < 3) // 1 2 --- 포탈/보스 상점
            {
                roomDataIndex = 0;
            }
            
            GameObject go = Instantiate(roomDataList[roomDataIndex].gameObject, stageGrid.transform);
            go.transform.localPosition = new Vector3(roomList[roomListIndex].x, roomList[roomListIndex].y, 0) * 100;
            RoomData rd = go.GetComponent<RoomData>();
            roomList[roomListIndex].roomData = rd;

            // # 열어야 할 문 열기
            if (roomList[roomListIndex].isDoorOpen[0]) rd.isDoorOpen[0] = true;
            else rd.isDoorOpen[0] = false;
            if (roomList[roomListIndex].isDoorOpen[1]) rd.isDoorOpen[1] = true;
            else rd.isDoorOpen[1] = false;
            if (roomList[roomListIndex].isDoorOpen[2]) rd.isDoorOpen[2] = true;
            else rd.isDoorOpen[2] = false;
            if (roomList[roomListIndex].isDoorOpen[3]) rd.isDoorOpen[3] = true;
            else rd.isDoorOpen[3] = false;

            // # 방 딕셔너리에 방 리스트 먼저 추가하고, 삭제하기
            roomDictionary.Add(new Vector2(roomList[roomListIndex].x, roomList[roomListIndex].y), roomList[roomListIndex]);
            roomList.RemoveAt(roomListIndex);
            roomDataList.RemoveAt(roomDataIndex);
        }

        StartCoroutine(GameManager.Instance.StartStage(spawnRoomCoordinate, roomDictionary));
    }

    private void PrintRoomMolds()
    {
        string s = "";

        // X축 번호 위쪽
        s += "A _ ";
        for (int i = 0; i < roomLength; i++) s += i + " ";

        // Y축 번호 왼쪽, 방 그릇 유무, Y축 번호 오른쪽
        s += "\n";
        for (int j = roomLength - 1; j >= 0; j--)
        {
            s += j + " _ ";
            for (int k = 0; k < roomLength; k++)
            {
                if(rooms[k, j] != null) s += "O ";
                else s += "X ";
            }
            s += " " + j + "\n";
        }
        
        // X축 번호 아래쪽
        s += "A _ ";
        for (int l = 0; l < roomLength; l++) s += l + " ";
            
        Debug.Log(s);
    }

    public void DestroyStage()
    {
        
    }
}

