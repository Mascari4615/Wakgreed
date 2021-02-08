using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Cinemachine;

public enum TravellerStat
{
    AD,
    AS
}

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    [HideInInspector] public static GameManager Instance { get { return instance; } }

    public int nyang { get; private set; } = 0;
    public int monsterKill { get; private set; } = 0;

    [HideInInspector] public bool isFighting = false;
    private int currentStageID = -1;
    [SerializeField] private EnemyRunTimeSet monsters;
    [SerializeField] private StageDataBuffer stageDataBuffer;

    [SerializeField] private int roomCount = 0;
    [SerializeField] private int stageEdgeLength = 0;
    private Dictionary<Vector2, Room> roomDictionary = new Dictionary<Vector2, Room>();
    private List<RoomMold> roomMolds = new List<RoomMold>();
    private class RoomMold
    {
        public Vector2 coordinate { get; set; }
        public bool[] isConnectToNearbyRoom = {false, false, false, false};
    }
    private List<Room> roomDatas = new List<Room>();
    private Stack<RoomMoldStack> roomMoldStackStack = new Stack<RoomMoldStack>();
    private class RoomMoldStack
    {
        public RoomMold originalRoomMold { get; set; }
        public Vector2 totalRoomMoldCoordinante { get; set; }
        public int originalRoomDoorIndex { get; set; }
        public int totalRoomDoorIndex { get; set; }
    }
    public Room currentRoom { get; private set; }
    [SerializeField] private GameObject stageGrid;
    
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private Text monsterKillText;
    [SerializeField] private Text nyangText;
    [SerializeField] private GameObject miniMapCamera;

    [SerializeField] private GameObject gameOverPanel; 

    [SerializeField] private GameObject pausePanel;
    
    [SerializeField] private GameObject mapPanel;
    [SerializeField] private GridLayoutGroup mapGridLayoutGroup;
    [SerializeField] private RectTransform scrollRectBackGround; 
    private Dictionary<Vector2, GameObject> roomUiDictionary = new Dictionary<Vector2, GameObject>();  

    [SerializeField] private GameObject fadePanel;
    [SerializeField] private Animator fadePanelAnimator;

    [SerializeField] private GameObject bagPanel;

    [SerializeField] private GameObject stageSpeedWagon;
    [SerializeField] private Text stageNumberText, stageNameText, stageCommentText;
    [SerializeField] private GameObject bossSpeedWagon;
    [SerializeField] private GameObject roomClearSpeedWagon;
    [SerializeField] private Text noticeText;

    [SerializeField] private GameObject undo;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        // 뒤로가기 버튼을 눌렀을 때, 정지 및 재개 ★ Time을 통한 실질적인 게임 정지 및 재개
        // # 정지 > 귀환 (StopCoroutine) 오류 가능성 : 따라서 플래그를 통해 현재 정지 시킬 수 있는지 확인해야함
        if (Input.GetKeyDown(KeyCode.Escape)) PauseGame();
    }

    public void PauseGame()
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
        Debug.Log("GenerateStage ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ---- ----");
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
            Debug.Log(item.coordinate);
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

        for (int i = 0; i < mapGridLayoutGroup.transform.childCount; i++)
        {
            if (i <= stageEdgeLength * stageEdgeLength - 1)
                mapGridLayoutGroup.transform.GetChild(i).gameObject.SetActive(true);
            else if (i > stageEdgeLength * stageEdgeLength - 1)
                mapGridLayoutGroup.transform.GetChild(i).gameObject.SetActive(false);

            for (int j = 0; j < mapGridLayoutGroup.transform.GetChild(i).transform.childCount; j++)
                mapGridLayoutGroup.transform.GetChild(i).transform.GetChild(j).gameObject.SetActive(false);
        }

        int childIndex = 0;
        for (int y = (stageEdgeLength - 1) / 2; y >= -(stageEdgeLength - 1) / 2; y--)
        {
            for (int x = -(stageEdgeLength - 1) / 2; x <= (stageEdgeLength - 1) / 2; x++, childIndex++)
            {
                roomUiDictionary.Add(new Vector2(x, y), mapGridLayoutGroup.transform.GetChild(childIndex).gameObject);
            }
        }

        UpdateMap();
    }

    private void UpdateMap()
    {  
        scrollRectBackGround.localPosition = -currentRoom.coordinate * 175;
        roomUiDictionary[currentRoom.coordinate].GetComponent<Image>().enabled = true;
        roomUiDictionary[currentRoom.coordinate].transform.Find("CurrentRoom").gameObject.SetActive(true);
        roomUiDictionary[currentRoom.coordinate].transform.Find("StageTheme").gameObject.SetActive(true);

        for (int i = 0; i < 4; i++)
        {
            Vector2 totalCoordinate = currentRoom.coordinate;
            string originDoor = "";
            string totalDoor = "";
     
            if (i == 0) {totalCoordinate.y++; originDoor = "Up"; totalDoor = "Down";}
            else if (i == 1) {totalCoordinate.y--; originDoor = "Down"; totalDoor = "Up";}
            else if (i == 2) {totalCoordinate.x--; originDoor = "Left"; totalDoor = "Right";}
            else if (i == 3) {totalCoordinate.x++; originDoor = "Right"; totalDoor = "Left";}

            Debug.Log($"{currentRoom.coordinate}, {totalCoordinate}, {i}");

            if (currentRoom.isConnectToNearbyRoom[i])
            {  
                roomUiDictionary[currentRoom.coordinate].transform.Find(originDoor).gameObject.SetActive(true);
                roomUiDictionary[totalCoordinate].GetComponent<Image>().enabled = true;
                roomUiDictionary[totalCoordinate].transform.Find(totalDoor).gameObject.SetActive(true);

                if (roomDictionary[totalCoordinate].roomType == RoomType.Boss)
                    roomUiDictionary[totalCoordinate].transform.Find("Boss").gameObject.SetActive(true);
                else if (roomDictionary[totalCoordinate].roomType == RoomType.Spawn)
                    roomUiDictionary[totalCoordinate].transform.Find("Spawn").gameObject.SetActive(true);
            }
        }
    }

    public IEnumerator MigrateRoom(Vector2 moveDirection, int totalDoorIndex)
    {
        fadePanel.SetActive(true);
        yield return new WaitForSeconds(0.2f);

        roomUiDictionary[currentRoom.coordinate].transform.Find("CurrentRoom").gameObject.SetActive(false);

        currentRoom = roomDictionary[currentRoom.coordinate + moveDirection];
        TravellerController.Instance.transform.position = new Vector3(currentRoom.doors[totalDoorIndex].transform.position.x, currentRoom.doors[totalDoorIndex].transform.position.y, 0) + (Vector3)moveDirection * 2f;
        miniMapCamera.transform.position = new Vector3(currentRoom.coordinate.x, currentRoom.coordinate.y, -1) * 100;

        UpdateMap();

        StopAllSpeedWagons();

        if (currentRoom.isCleared == false)
        {
            bagPanel.SetActive(false);
            mapPanel.SetActive(false);  
        }
        currentRoom.Enter();
        
        fadePanelAnimator.SetTrigger("FadeIn");

        yield return new WaitForSeconds(0.2f);
        fadePanel.SetActive(false);
    }

    public IEnumerator GameOver()
    {
        Debug.Log("GameOver");
        // 플레이어 스크립트에서 hp <= 0, Died 감지 > 마지막 처리 후 플레이어 스크립트 비활성화
        yield return new WaitForSeconds(2f); // 2초 동안 Player Died/Recall 애니메이션 실행

        gamePanel.SetActive(false); // 게임 Panel @비활성화
        gameOverPanel.SetActive(true); // 게임 결과 Panel @활성화
    }
    
    public void Recall()
    {
        DestroyStage();
        undo.SetActive(true);
        currentStageID = -1;

        StopAllSpeedWagons();

        AbilityManager.Instance.selectAbilityPanel.SetActive(false);

        ObjectManager.Instance.InsertAll();
        // UpdateMap();
        monsters.Items.Clear();

        TravellerController.Instance.enabled = true;

        miniMapCamera.transform.position = new Vector3(0, 0, -100);

        isFighting = false;
        pausePanel.SetActive(false);
        gameOverPanel.SetActive(false);
        gamePanel.SetActive(true);

        Time.timeScale = 1;
    }

    public void AcquireNyang(int amount)
    {
        nyang += amount;
        nyangText.text = nyang.ToString();
    }

    public void AcquireKillCount()
    {
        monsterKill++;
        monsterKillText.text = monsterKill.ToString();
    }

    public void OpenAndCloseBag()
    {
        if (isFighting)
        {
            StopCoroutine(NoticeText("전투 중에는 열 수 없습니다.", 1.5f));
            StartCoroutine(NoticeText("전투 중에는 열 수 없습니다.", 1.5f));
        }
        else
        {
            StopCoroutine(NoticeText("전투 중에는 열 수 없습니다.", 1.5f));

            bagPanel.SetActive(!bagPanel.activeSelf);
        }
    }

    public void OpenAndCloseMap()
    {
        if (isFighting)
        {
            StopCoroutine(NoticeText("전투 중에는 열 수 없습니다.", 1.5f));
            StartCoroutine(NoticeText("전투 중에는 열 수 없습니다.", 1.5f));
        }
        else
        {
            StopCoroutine(NoticeText("전투 중에는 열 수 없습니다.", 1.5f));

            scrollRectBackGround.localPosition = -currentRoom.coordinate * 175;
            mapPanel.SetActive(!mapPanel.activeSelf);
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
        stageNameText.text = $"::{stageDataBuffer.Items[currentStageID].name}::";
        stageCommentText.text= $"::{stageDataBuffer.Items[currentStageID].comment}::";
        yield return new WaitForSeconds(2f);
        stageSpeedWagon.SetActive(false);
    }

    public IEnumerator BossSpeedWagon()
    {
        GameObject.Find("SlimeKing(Clone)").GetComponent<SlimeKing>().enabled = false;
        GameObject.Find("SlimeKing(Clone)").transform.Find("CM Camera1").GetComponent<CinemachineVirtualCamera>().Priority = 100;
        TravellerController.Instance.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        bossSpeedWagon.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(3f);
        bossSpeedWagon.gameObject.SetActive(false);
        TravellerController.Instance.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        GameObject.Find("SlimeKing(Clone)").transform.Find("CM Camera1").GetComponent<CinemachineVirtualCamera>().Priority = 0;
        GameObject.Find("SlimeKing(Clone)").GetComponent<SlimeKing>().enabled = true;
    }

    public IEnumerator RoomClearSpeedWagon()
    {
        roomClearSpeedWagon.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(2f);
        roomClearSpeedWagon.gameObject.SetActive(false);
    }

    private IEnumerator NoticeText(string text, float time)
    {
        noticeText.text = text;
        noticeText.gameObject.SetActive(true);
        yield return new WaitForSeconds(time);
        noticeText.gameObject.SetActive(false);
    }

    private void StopAllSpeedWagons()
    {
        StopCoroutine("BossSpeedWagon");
        bossSpeedWagon.SetActive(false);
        StopCoroutine("StageSpeedWagon");
        stageSpeedWagon.SetActive(false);
        StopCoroutine("RoomClearSpeedWagon");
        roomClearSpeedWagon.SetActive(false);
        StopCoroutine(NoticeText("전투 중에는 열 수 없습니다.", 1.5f));
        noticeText.gameObject.SetActive(false);
    }
}