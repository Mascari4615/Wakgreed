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
        if (Random.Range(0, 2) == 0 && y != StageManager.Instace.roomMoldLength - 1) StageManager.Instace.roomStackQueue.Enqueue(new RoomStack(x, y, "Up"));
        if (Random.Range(0, 2) == 0 && y != 0) StageManager.Instace.roomStackQueue.Enqueue(new RoomStack(x, y, "Down"));
        if (Random.Range(0, 2) == 0 && x != 0) StageManager.Instace.roomStackQueue.Enqueue(new RoomStack(x, y, "Left"));
        if (Random.Range(0, 2) == 0 && x != StageManager.Instace.roomMoldLength - 1) StageManager.Instace.roomStackQueue.Enqueue(new RoomStack(x, y, "Right"));
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

    [SerializeField] private GameObject[] stagePrefab = new GameObject[0];
    public int maxRoomCount = 0;
    public int roomMoldLength = 0;

    [HideInInspector] public GameObject stage;
    [HideInInspector] public StageData stageData;
    private RoomMold[,] roomMolds = new RoomMold[0, 0];
    private int roomCount = 0;
    [HideInInspector] public Queue<RoomStack> roomStackQueue = new Queue<RoomStack>();
    private RoomData[] roomDatas = new RoomData[0]; 
    private List<RoomData> roomDataList = new List<RoomData>();
    private List<RoomMold> roomList = new List<RoomMold>();
    public Dictionary<int[], RoomMold> roomDictionary = new Dictionary<int[], RoomMold>();

    void Awake()
    {
        instance = this;
    }

    void Initialize()
    {
        if (stage != null)
            Destroy(stage.gameObject);

        stage = null;
        stageData = null;
        roomMolds.Initialize();
        roomMolds = new RoomMold[roomMoldLength, roomMoldLength];
        roomCount = 0;
        roomStackQueue.Clear();
        roomDatas.Initialize();
        roomDataList.Clear();
        roomList.Clear();
        roomDictionary.Clear();
    }

    public void GenerateStage(int currentStageNumber) // 스테이지 만들기 시작
    {
        Initialize();

        stage = Instantiate(stagePrefab[currentStageNumber]);
        stageData = stage.GetComponent<StageData>();
        roomDatas = stageData.roomDatas;

        roomDataList = new List<RoomData>(roomDatas);
        // 방 데이터 리스트에, 현재 스테이지의 방 데이터 배열을 대입

        // (Length - 1) / 2 안하고 Length / 2 하는 이유 : 
        // 예를 들어 Length 3이면 전자의 경우 2, 후자의 경우 1.5 가 되는데, 배열은 0부터 시작하니까 가운데가 [1, 1]임. 근데 전자 즉 [2, 2] 는 아니고, 배열 index는 정수라 1.5라는 값이 들어가면 1로 바뀌어서 [1, 1]이 되버림.

        // 가운데에 가장 기본 방 생성, 방 리스트에 넣기
        roomMolds[roomMoldLength / 2, roomMoldLength / 2] = new RoomMold(roomMoldLength / 2, roomMoldLength / 2);
        roomList.Add(roomMolds[roomMoldLength / 2, roomMoldLength / 2]);
        roomCount = maxRoomCount - 1; // 최대로 생성할 방 개수 설정, 위에서 하나 생성했으니 -1
        
        GenerateRoom(); // 방 생성 시작
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

            if (roomMolds[stack.totalX, stack.totalY] != null) continue;
            
            roomMolds[stack.totalX, stack.totalY] = new RoomMold(stack.totalX, stack.totalY);
            int origin = 0;
            int total = 0;;

            if (stack.direction == "Up") {origin = 0; total = 1;}
            else if (stack.direction == "Down") {origin = 1; total = 0;}
            else if (stack.direction == "Left") {origin = 2; total = 3;}
            else if (stack.direction == "Right") {origin = 3; total = 2;}
    
            roomMolds[stack.originalX, stack.originalY].isDoorOpen[origin] = true;
            roomMolds[stack.totalX, stack.totalY].isDoorOpen[total] = true;

            roomList.Add(roomMolds[stack.totalX, stack.totalY]);
            roomCount--;
        }

        PrintRoomMolds();

        int roomListCount = roomList.Count;
        int[] spawnRoomCoordinate = new int[]{};

        for (int i = 0; i < roomListCount; i++) // 랜덤한 방 그릇에 랜덤한 방 데이터 대입
        {      
            int roomListIndex = Random.Range(0, roomList.Count); // 방 구조용
            int roomDataIndex = Random.Range(0, roomDataList.Count); // 방 데이터용

            if (i == 0) // 0 --- 스폰
            {
                roomListIndex = 0;
                roomDataIndex = 0;;
                roomList[roomListIndex].roomData = roomDataList[roomDataIndex];
                spawnRoomCoordinate = new int[]{roomList[roomListIndex].x, roomList[roomListIndex].y};
            }
            else if (i < 3) // 1 2 --- 포탈/보스 상점
            {
                roomDataIndex = 0;
                roomList[roomListIndex].roomData = roomDataList[roomDataIndex];
            }
            else // 나머지
            {
                roomList[roomListIndex].roomData = roomDataList[roomDataIndex];
            }

            // # 열어야 할 문 열기
            if (roomList[roomListIndex].isDoorOpen[0]) roomList[roomListIndex].roomData.isDoorOpen[0] = true;
            if (roomList[roomListIndex].isDoorOpen[1]) roomList[roomListIndex].roomData.isDoorOpen[1] = true;
            if (roomList[roomListIndex].isDoorOpen[2]) roomList[roomListIndex].roomData.isDoorOpen[2] = true;
            if (roomList[roomListIndex].isDoorOpen[3]) roomList[roomListIndex].roomData.isDoorOpen[3] = true;

            // # 방 딕셔너리에 방 리스트 먼저 추가하고, 삭제하기
            roomDictionary.Add(new int[]{roomList[roomListIndex].x, roomList[roomListIndex].y}, roomList[roomListIndex]);
            Debug.Log(roomDictionary[new int[]{roomList[roomListIndex].x, roomList[roomListIndex].y}]);
            roomList.RemoveAt(roomListIndex);
            roomDataList.RemoveAt(roomDataIndex);
        }
        StartCoroutine(GameManager.Instance.StartStage(spawnRoomCoordinate));
    }

    private void PrintRoomMolds()
    {
        string s = "";

        // X축 번호 위쪽
        s += "A _ ";
        for (int i = 0; i < roomMoldLength; i++) s += i + " ";

        // Y축 번호 왼쪽, 방 그릇 유무, Y축 번호 오른쪽
        s += "\n";
        for (int j = roomMoldLength - 1; j >= 0; j--)
        {
            s += j + " _ ";
            for (int k = 0; k < roomMoldLength; k++)
            {
                if(roomMolds[k, j] != null) s += "O ";
                else s += "X ";
            }
            s += " " + j + "\n";
        }
        
        // X축 번호 아래쪽
        s += "A _ ";
        for (int l = 0; l < roomMoldLength; l++) s += l + " ";
            
        Debug.Log(s);
    }
}

