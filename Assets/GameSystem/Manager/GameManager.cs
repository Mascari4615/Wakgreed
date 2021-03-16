using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    [HideInInspector] public static GameManager Instance { get { return instance; } }

    private int monsterKill = 0;
    private bool isFighting = false;
    public void SetFighting(bool value)
    {
        isFighting = value;
        if (isFighting == true) OnFightStart.Raise();
        else if (isFighting == false) OnFightEnd.Raise();
    }
    [SerializeField] private GameEvent OnFightStart;
    [SerializeField] private GameEvent OnFightEnd;
    [SerializeField] private GameEvent OnRecall;
    [SerializeField] private MasteryManager MasteryManager;
    [SerializeField] private BuffRunTimeSet buffRunTimeSet;

    private int currentStageID = -1;
    [SerializeField] private StageDataBuffer stageDataBuffer;
    [SerializeField] EnemyRunTimeSet EnemyRunTimeSet;

    [SerializeField] private int roomCount = 0;
    [SerializeField] private int stageEdgeLength = 0;
    private Dictionary<Vector2, Room> roomDictionary = new Dictionary<Vector2, Room>();
    private List<RoomMold> roomMolds = new List<RoomMold>();
    private class RoomMold
    {
        public Vector2 coordinate;
        public bool[] isConnectToNearbyRoom = new bool[4];
    }
    private List<Room> roomDatas = new List<Room>();
    private Stack<RoomMoldStack> roomMoldStackStack = new Stack<RoomMoldStack>();
    private struct RoomMoldStack
    {
        public RoomMold originalRoomMold;
        public Vector2 totalRoomMoldCoordinante;
        public int originalRoomDoorIndex;
        public int totalRoomDoorIndex;
    }
    public Room currentRoom { get; private set; }
    [SerializeField] private GameObject stageGrid;
    
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private Text monsterKillText;
    [SerializeField] private GameObject miniMapCamera;

    [SerializeField] private GameObject gameOverPanel; 

    [SerializeField] private GameObject pausePanel;
    
    [SerializeField] private GameObject mapPanel;
    [SerializeField] private GridLayoutGroup mapGridLayoutGroup;
    [SerializeField] private RectTransform scrollRectBackGround; 
    private Dictionary<Vector2, GameObject> roomUiDictionary = new Dictionary<Vector2, GameObject>();  

    [SerializeField] private GameObject fadePanel;
    [SerializeField] private Animator fadePanelAnimator;

    [SerializeField] private GameObject stageSpeedWagon;
    [SerializeField] private TextMeshProUGUI stageNumberText, stageNameCommentText;
    [SerializeField] private GameObject roomClearSpeedWagon;
    [SerializeField] private TextMeshProUGUI noticeText;

    [SerializeField] private GameObject undo;

    private void Awake()
    {
        instance = this;

        StartCoroutine(CheckBuff());
    }

    private IEnumerator CheckBuff()
    {
        while (true)
        {
            foreach (var buff in buffRunTimeSet.Items)
            {
                // Debug.Log(buff.name);
                if (buff.hasCondition) continue;
                else if (buff.removeTime >= Time.time) {buff.OnRemove(); buffRunTimeSet.Remove(buff);}
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    private void Update()
    {
        // 뒤로가기 버튼을 눌렀을 때, 정지 및 재개 ★ Time을 통한 실질적인 게임 정지 및 재개
        // # 정지 > 귀환 (StopCoroutine) 오류 가능성 : 따라서 플래그를 통해 현재 정지 시킬 수 있는지 확인해야함
        if (Input.GetKeyDown(KeyCode.Escape)) PauseGame();
        if (Input.GetKeyDown(KeyCode.Tab) && mapPanel.activeSelf == false) MapDoor(true);
        else if (Input.GetKeyUp(KeyCode.Tab) && mapPanel.activeSelf == true) MapDoor(false);
    }

    public void PauseGame() // 정지 버튼에서 호출
    {
        if (Time.timeScale == 1) Time.timeScale = 0;
        else if (Time.timeScale == 0) Time.timeScale = 1;

        mapPanel.SetActive(false);
        pausePanel.SetActive(!pausePanel.activeSelf);
    }

    public IEnumerator EnterPortal()
    {
        fadePanel.SetActive(true);
        yield return new WaitForSeconds(0.2f);

        undo.SetActive(false);

        currentStageID++;
        GenerateStage(currentStageID);
    }

    private void GenerateStage(int stageIndex)
    {
        Debug.Log("GenerateStage");
        DestroyStage();
        roomMolds.Clear();
        roomDatas = new List<Room>(stageDataBuffer.Items[stageIndex].roomDatas);
        roomMoldStackStack.Clear();
        roomDictionary.Clear();

        roomMolds.Add(new RoomMold(){coordinate = Vector2.zero});

        while (roomMolds.Count < roomCount)
        {
            if (roomMoldStackStack.Count == 0) 
            {
                GenerateRoomMoldStack(roomMolds[Random.Range(0, roomMolds.Count)]);
                continue;
            }

            RoomMoldStack roomMoldStack = roomMoldStackStack.Pop();
            
            foreach (RoomMold roomMold in roomMolds)
                if (roomMold.coordinate == roomMoldStack.totalRoomMoldCoordinante)
                    goto CONTINUE;
            RoomMold totalRoomMold = new RoomMold(){coordinate = roomMoldStack.totalRoomMoldCoordinante};
            RoomMold originalRoomMold = roomMoldStack.originalRoomMold;
            
            originalRoomMold.isConnectToNearbyRoom[roomMoldStack.originalRoomDoorIndex] = true;
            totalRoomMold.isConnectToNearbyRoom[roomMoldStack.totalRoomDoorIndex] = true;
            
            // Debug.Log($"roomMoldsAdd : {totalRoomMold.coordinate}");
            roomMolds.Add(totalRoomMold);
            GenerateRoomMoldStack(totalRoomMold);

            CONTINUE:;
        }

        foreach (var item in roomMolds)
        {
            // Debug.Log(item.coordinate);
        }
        
        for (int i = 0; i < roomCount; i++)
        {      
            int roomMoldIndex = (i == 0) ? 0 : Random.Range(0, roomMolds.Count);
            int roomDataIndex = (i <= 1) ? 0 : Random.Range(0, roomDatas.Count);
            // Debug.Log($"roomMolds : {roomMolds.Count} - {roomMoldIndex}, roomDatas : {roomDatas.Count} - {roomDataIndex}");

            Room r = Instantiate(roomDatas[roomDataIndex].gameObject, stageGrid.transform).GetComponent<Room>();
            r.Initialize(roomMolds[roomMoldIndex].coordinate, roomMolds[roomMoldIndex].isConnectToNearbyRoom);
            
            // Debug.Log($"i : {i}, roomMold : {roomMolds[roomMoldIndex].coordinate}, r : {r.coordinate}");
            roomMolds.RemoveAt(roomMoldIndex);
            roomDatas.RemoveAt(roomDataIndex);
            roomDictionary.Add(r.coordinate, r);
        }

        StartCoroutine(StartStage());
    }

    private void GenerateRoomMoldStack(RoomMold originalRoomMold)
    {
        for (int i = 0; i < 4; i++)
        {
            Vector2 totalRoomMoldCoordinante = originalRoomMold.coordinate + 
            ((i == 0) ? Vector2.up : (i == 1) ? Vector2.down : (i == 2) ? Vector2.left : Vector2.right);

            if (Random.Range(0, 2) == 0) continue;

            if (i == 0 && originalRoomMold.coordinate.y == (stageEdgeLength - 1) / 2) continue;
            else if (i == 1 && originalRoomMold.coordinate.y == -(stageEdgeLength - 1) / 2) continue;
            else if (i == 2 && originalRoomMold.coordinate.x == -(stageEdgeLength - 1) / 2) continue;
            else if (i == 3 && originalRoomMold.coordinate.x == (stageEdgeLength - 1) / 2) continue;

            roomMoldStackStack.Push(new RoomMoldStack(){originalRoomMold = originalRoomMold, totalRoomMoldCoordinante = totalRoomMoldCoordinante, 
            originalRoomDoorIndex = i, totalRoomDoorIndex = (i == 0) ? 1 : (i == 1) ? 0 : (i == 2) ? 3 : 2});
        }    
    }

    private void DestroyStage()
    {
        for (int i = 0; i < stageGrid.transform.childCount; i++)
        {
            Destroy(stageGrid.transform.GetChild(i).gameObject);
        }
    }

    private IEnumerator StartStage()
    {  
        currentRoom = roomDictionary[Vector2.zero];
        currentRoom.Enter();
        InitialzeMap();
        
        TravellerController.Instance.transform.position = new Vector3(currentRoom.coordinate.x, currentRoom.coordinate.y, 0) * 100;
        miniMapCamera.transform.position = new Vector3(currentRoom.coordinate.x, currentRoom.coordinate.y, -1) * 100;

        fadePanelAnimator.SetTrigger("FadeIn");
        StartCoroutine("StageSpeedWagon");
        yield return new WaitForSeconds(0.2f);
        fadePanel.SetActive(false);
    }

    private void InitialzeMap()
    {     
        mapGridLayoutGroup.constraintCount = stageEdgeLength;
        roomUiDictionary = new Dictionary<Vector2, GameObject>();

        int x = -(stageEdgeLength - 1) / 2;
        int y = (stageEdgeLength - 1) / 2;
        for (int i = 0; i < mapGridLayoutGroup.transform.childCount; i++)
        {
            if (i <= stageEdgeLength * stageEdgeLength - 1)
            {
                GameObject targetRoomUI = mapGridLayoutGroup.transform.GetChild(i).gameObject;
                Vector2 targetRoomCoordinate = new Vector2(x, y);
                // Debug.Log(targetRoomCoordinate);
                targetRoomUI.SetActive(true);
                targetRoomUI.GetComponent<Image>().enabled = false;
                targetRoomUI.transform.Find("CurrentRoom").gameObject.SetActive(false);
                targetRoomUI.transform.GetChild(0).gameObject.SetActive(false);

                if (roomDictionary.ContainsKey(targetRoomCoordinate))
                {
                    Room targetRoom = roomDictionary[targetRoomCoordinate];
                    roomUiDictionary.Add(targetRoomCoordinate, targetRoomUI);
                    targetRoomUI.transform.GetChild(0).Find("Boss").gameObject.SetActive(targetRoom.roomType == RoomType.Boss);
                    targetRoomUI.transform.GetChild(0).Find("Spawn").gameObject.SetActive(targetRoom.roomType == RoomType.Spawn);
                    targetRoomUI.transform.GetChild(0).Find("Up").gameObject.SetActive(targetRoom.isConnectToNearbyRoom[0]);
                    targetRoomUI.transform.GetChild(0).Find("Down").gameObject.SetActive(targetRoom.isConnectToNearbyRoom[1]);
                    targetRoomUI.transform.GetChild(0).Find("Left").gameObject.SetActive(targetRoom.isConnectToNearbyRoom[2]);
                    targetRoomUI.transform.GetChild(0).Find("Right").gameObject.SetActive(targetRoom.isConnectToNearbyRoom[3]);
                }

                x++;
                if (x > (stageEdgeLength - 1) / 2)
                {
                    x = -(stageEdgeLength - 1) / 2;
                    y--;
                }
            }
            else if (i > stageEdgeLength * stageEdgeLength - 1)
            {
                mapGridLayoutGroup.transform.GetChild(i).gameObject.SetActive(false);
            }
        }

        UpdateMap();
    }

    private void UpdateMap()
    {  
        scrollRectBackGround.localPosition = -currentRoom.coordinate * 175;
        roomUiDictionary[currentRoom.coordinate].GetComponent<Image>().enabled = true;
        roomUiDictionary[currentRoom.coordinate].transform.GetChild(0).gameObject.SetActive(true);
        roomUiDictionary[currentRoom.coordinate].transform.Find("CurrentRoom").gameObject.SetActive(true);

        if (currentRoom.isConnectToNearbyRoom[0])
            roomUiDictionary[currentRoom.coordinate + Vector2.up].GetComponent<Image>().enabled = true;
        if (currentRoom.isConnectToNearbyRoom[1])
            roomUiDictionary[currentRoom.coordinate + Vector2.down].GetComponent<Image>().enabled = true;
        if (currentRoom.isConnectToNearbyRoom[2])
            roomUiDictionary[currentRoom.coordinate + Vector2.left].GetComponent<Image>().enabled = true;
        if (currentRoom.isConnectToNearbyRoom[3])
            roomUiDictionary[currentRoom.coordinate + Vector2.right].GetComponent<Image>().enabled = true;
    }

    public IEnumerator MigrateRoom(Vector2 moveDirection, int spawnDirection)
    {
        fadePanel.SetActive(true);
        yield return new WaitForSeconds(0.2f);

        roomUiDictionary[currentRoom.coordinate].transform.Find("CurrentRoom").gameObject.SetActive(false);

        currentRoom = roomDictionary[currentRoom.coordinate + moveDirection];
        TravellerController.Instance.transform.position = currentRoom.doors[spawnDirection].transform.position + (Vector3)moveDirection * 2;
        miniMapCamera.transform.position = new Vector3(currentRoom.coordinate.x, currentRoom.coordinate.y, -1) * 100;

        UpdateMap();
        StopAllSpeedWagons();

        if (currentRoom.isVisited == false)
        {
            mapPanel.SetActive(false);  
        }
        currentRoom.Enter();
        
        fadePanelAnimator.SetTrigger("FadeIn");

        yield return new WaitForSeconds(0.2f);
        fadePanel.SetActive(false);
    }

    public void GameOver()
    {
        Debug.Log("GameOver");

        gamePanel.SetActive(false);
        gameOverPanel.SetActive(true);
    }
    
    public void Recall()
    {
        OnRecall.Raise();
        DestroyStage();
        undo.SetActive(true);
        currentStageID = -1;

        StopAllSpeedWagons();

        MasteryManager.selectMasteryPanel.SetActive(false);

        // UpdateMap();
        int count = EnemyRunTimeSet.Items.Count;
        for (int i = 0; i < count; i++)
        {
            EnemyRunTimeSet.Items[0].SetActive(false);
            EnemyRunTimeSet.Remove(EnemyRunTimeSet.Items[0]);
        }

        TravellerController.Instance.enabled = true;
        TravellerController.Instance.Initialize();

        miniMapCamera.transform.position = new Vector3(0, 0, -100);

        isFighting = false;
        pausePanel.SetActive(false);
        gameOverPanel.SetActive(false);
        gamePanel.SetActive(true);

        Time.timeScale = 1;
    }

    public void AcquireKillCount()
    {
        monsterKill++;
        monsterKillText.text = monsterKill.ToString();
    }

    private void MapDoor(bool bOpen)
    {
        if (isFighting)
        {
            StopCoroutine("CantOpenText");
            StartCoroutine("CantOpenText");
        }
        else
        {
            StopCoroutine("CantOpenText");

            scrollRectBackGround.localPosition = -currentRoom.coordinate * 175;
            mapPanel.SetActive(bOpen);
        }    
    }

    public void QuitGame()
    { 
        Debug.Log("Quitting Game...");
        Application.Quit();
    }

    private IEnumerator StageSpeedWagon()
    {
        stageSpeedWagon.SetActive(true);
        stageNumberText.text = $"1-{stageDataBuffer.Items[currentStageID].id}";
        stageNameCommentText.text = $"{stageDataBuffer.Items[currentStageID].name} : {stageDataBuffer.Items[currentStageID].comment}";
        yield return new WaitForSeconds(2f);
        stageSpeedWagon.SetActive(false);
    }

    public IEnumerator RoomClearSpeedWagon()
    {
        roomClearSpeedWagon.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        roomClearSpeedWagon.gameObject.SetActive(false);
    }

    private IEnumerator CantOpenText()
    {
        noticeText.text = "전투 중에는 열 수 없습니다.";
        noticeText.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        noticeText.gameObject.SetActive(false);
    }

    private void StopAllSpeedWagons()
    {
        StopCoroutine("StageSpeedWagon");
        stageSpeedWagon.SetActive(false);
        StopCoroutine("RoomClearSpeedWagon");
        roomClearSpeedWagon.SetActive(false);
        StopCoroutine("CantOpenText");
        noticeText.gameObject.SetActive(false);
    }
}