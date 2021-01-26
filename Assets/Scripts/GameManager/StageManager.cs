using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class RoomMold
{
    public RoomData roomData = new RoomData();

    public int x, y;
    private bool wantsUpperRoom, wantsLowerRoom, wantsLeftRoom, wantsRightRoom = false;
    public bool[] isDoorOpen = new bool[4];

    public RoomMold(int _x, int _y) // 선언과 동시에 실행되는 함수
    {
        x = _x;
        y = _y;

        GenerateRoomStack();
    }

    public void GenerateRoomStack()
    {
        if (Random.Range(0, 2) == 0 && y != StageManager.Instace.roomMoldLength - 1) wantsUpperRoom = true;
        if (Random.Range(0, 2) == 0 && y != 0) wantsLowerRoom = true;
        if (Random.Range(0, 2) == 0 && x != 0) wantsLeftRoom = true;
        if (Random.Range(0, 2) == 0 && x != StageManager.Instace.roomMoldLength - 1) wantsRightRoom = true;
        
        if (wantsUpperRoom) StageManager.Instace.roomStackQueue.Enqueue(new RoomStack(x, y, "Up"));
        if (wantsLowerRoom) StageManager.Instace.roomStackQueue.Enqueue(new RoomStack(x, y, "Down"));
        if (wantsLeftRoom) StageManager.Instace.roomStackQueue.Enqueue(new RoomStack(x, y, "Left"));
        if (wantsRightRoom) StageManager.Instace.roomStackQueue.Enqueue(new RoomStack(x, y, "Right"));
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
        
        switch(direction)
        {
            case "Up" : totalY = y + 1; break;
            case "Down" : totalY = y - 1; break;
            case "Left" : totalX = x - 1; break;
            case "Right" : totalX = x + 1; break;
        }
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
    private RoomMold[,] roomMolds = new RoomMold[0,0];
    private int roomCount = 0;
    [HideInInspector] public Queue<RoomStack> roomStackQueue = new Queue<RoomStack>();
    private RoomData[] roomDatas = new RoomData[0]; 
    private List<RoomData> roomDataList = new List<RoomData>();
    private List<RoomMold> roomList = new List<RoomMold>();
    private Dictionary<string, RoomMold> roomDictionary = new Dictionary<string, RoomMold>();

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

        for (int i = 0; i < roomDatas.Length; i++) // 방 데이터 리스트에, 현재 스테이지의 방 데이터 배열을 대입
            roomDataList.Add(roomDatas[i]);

        // Debug.Log("----- Generating RoomMold Structure -----");
        roomMolds = new RoomMold[roomMoldLength, roomMoldLength]; // 방 구조를 저장할 배열 생성

        roomMolds[roomMoldLength / 2, roomMoldLength / 2] = new RoomMold(roomMoldLength / 2, roomMoldLength / 2); // 가운데에 가장 기본 방 생성
        roomList.Add(roomMolds[roomMoldLength / 2, roomMoldLength / 2]); // 방 리스트에 넣음
        roomCount = maxRoomCount - 1; // 최대로 생성할 방 개수 설정, 위에서 하나 생성했으니 -1
        // Debug.Log("First Room : " + roomMoldLength / 2 + "_" + roomMoldLength / 2);

        // Debug.Log("----- Start Coroutine GenerateRoom() -----");
        StartCoroutine("GenerateRoom"); // 방 생성 시작
    }

    IEnumerator GenerateRoom()
    {
        if (roomCount <= 0) // 만약 생성할 수 있는 방 개수가 0 이하면, 방 생성 멈추기 + 결과 보여주기
        { 
            // Debug.Log("----- Stop Corotine GeneratedRoom() -----");
            StopCoroutine("GenerateRoom");
            AfterRoomMoldsGenerated();
        }
        else
        {
            // Debug.Log("Try to Generate Room...");
            if (roomStackQueue.Count > 0)
            {
                RoomStack stack = roomStackQueue.Dequeue();
                if (roomMolds[stack.totalX, stack.totalY] == null)
                {
                    roomMolds[stack.totalX, stack.totalY] = new RoomMold(stack.totalX, stack.totalY);
                    switch (stack.direction)
                    {
                        case "Up" :
                            roomMolds[stack.originalX, stack.originalY].isDoorOpen[0] = true;
                            roomMolds[stack.totalX, stack.totalY].isDoorOpen[1] = true;
                            break;
                        
                        case "Down" :
                            roomMolds[stack.originalX, stack.originalY].isDoorOpen[1] = true;
                            roomMolds[stack.totalX, stack.totalY].isDoorOpen[0] = true;
                            break;

                        case "Left" :
                            roomMolds[stack.originalX, stack.originalY].isDoorOpen[2] = true;
                            roomMolds[stack.totalX, stack.totalY].isDoorOpen[3] = true;
                            break;

                        case "Right" :
                            roomMolds[stack.originalX, stack.originalY].isDoorOpen[3] = true;
                            roomMolds[stack.totalX, stack.totalY].isDoorOpen[2] = true;
                            break;
                    }
                    roomList.Add(roomMolds[stack.totalX, stack.totalY]);
                    roomCount--;
                    // Debug.Log("Success" + stack.totalX + "_" + stack.totalY);
                }
            }
            else
            {
                roomList[Random.Range(0, roomList.Count)].GenerateRoomStack();
            }
            yield return 0;
            StartCoroutine("GenerateRoom");
        }
    }

    private void AfterRoomMoldsGenerated() // 방 구조가 만들어진 후 실행
    {
        PrintRoomMolds();

        string spawnRoomCoordinate = null;
        int roomListCount = roomList.Count;

        // Debug.Log("RoomList Count : " + roomList.Count);
        // Debug.Log("RoomDataList Count : " + roomDataList.Count);
        for (int i = 0; i < roomListCount; i++) // 랜덤한 방 그릇에 랜덤한 방 데이터 대입
        {
            int roomIndex = Random.Range(0, roomList.Count); // 방 구조용
            int roomDataIndex = Random.Range(0, roomDataList.Count); // 방 데이터용

            if (i == 0) // 0 --- 스폰
            {
                roomIndex = 0;
                roomDataIndex = 0;

                roomList[roomIndex].roomData = roomDataList[roomDataIndex];
 
                spawnRoomCoordinate = roomList[roomIndex].x + "_" + roomList[roomIndex].y;
            }
            else if (i < 3) // 1 2 --- 포탈/보스 상점
            {
                roomDataIndex = 0;

                roomList[roomIndex].roomData = roomDataList[roomDataIndex];
            }
            else // 나머지
            {
                roomList[roomIndex].roomData = roomDataList[roomDataIndex];
            }
            
            // # 열어야 할 문 열기
            if (roomList[roomIndex].isDoorOpen[0])
                roomList[roomIndex].roomData.isDoorOpen[0] = true;
            if (roomList[roomIndex].isDoorOpen[1])
                roomList[roomIndex].roomData.isDoorOpen[1] = true;
            if (roomList[roomIndex].isDoorOpen[2])
                roomList[roomIndex].roomData.isDoorOpen[2] = true;
            if (roomList[roomIndex].isDoorOpen[3])
                roomList[roomIndex].roomData.isDoorOpen[3] = true;

            // # 방 딕셔너리에 방 리스트 추가하고, 삭제하기
            // Debug.Log(roomList[roomIndex].x + "_" +roomList[roomIndex].y); // # 생성된 방 체크
            roomDictionary.Add(roomList[roomIndex].x + "_" +roomList[roomIndex].y, roomList[roomIndex]);
            roomList.RemoveAt(roomIndex);
            roomDataList.RemoveAt(roomDataIndex);
        }
        // Debug.Log("Spawn Room : " + spawnRoomCoordinate);
        // Debug.Log("Dictionary Count : " + roomDictionary.Count);

        StartCoroutine(GameManager.Instance.StartStage(roomDictionary, spawnRoomCoordinate));
    }
    
    private void PrintRoomMolds()
    {
        string s = null;

        // X축 번호 위쪽
        s += "A _ ";
        for (int i = 0; i < roomMoldLength; i++)
            s += i + " ";

        // Y축 번호 왼쪽, 방 그릇 유무, Y축 번호 오른쪽
        s += "\n";
        for (int j = roomMoldLength - 1; j >= 0; j--)
        {
            s += j + " _ ";
            for (int k = 0; k < roomMoldLength; k++)
            {
                if(roomMolds[k, j] != null)
                    s += "O ";
                else
                    s += "X ";
            }
            s += " " + j + "\n";
        }
        
        // X축 번호 아래쪽
        s += "A _ ";
        for (int l = 0; l < roomMoldLength; l++)
            s += l + " ";
            
        // Debug.Log(s);
    }
}

