using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Cinemachine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    [HideInInspector] public static GameManager Instance { get { return instance; } }

    [HideInInspector] public int nyang = 0;
    [HideInInspector] public int monsterKill = 0;
    [HideInInspector] public bool isFighting = false;
    public int currentStageID = -1;

    private Dictionary<Vector2, RoomMold> roomDictionary = new Dictionary<Vector2, RoomMold>();
    [HideInInspector] public RoomMold currentRoom;
    
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private Text monsterKillText;
    [SerializeField] private Text nyangText;
    [SerializeField] private GameObject miniMapCamera;

    [SerializeField] private GameObject gameOverPanel; 

    [SerializeField] private GameObject pausePanel;
    
    [SerializeField] private GameObject mapPanel;
    [SerializeField] private ScrollRect scrollRect;
    [SerializeField] GameObject mapGridLayoutGroupPreFab;
    [SerializeField] GridLayoutGroup mapGridLayoutGroup;
    [SerializeField] GameObject roomUi;
    private GameObject[,] roomUiArray;   

    [SerializeField] private GameObject fadePanel;
    [SerializeField] private Animator fadePanelAnimator;

    [SerializeField] private GameObject bagPanel;

    [SerializeField] private Text noticeText;
    [HideInInspector] public List<GameObject> monsters;
    
    [SerializeField] private GameObject monsterPool;

    [SerializeField] private GameObject stageSpeedWagon = null;
    [SerializeField] private Text stageNumberText, stageNameText = null;
    [SerializeField] private GameObject bossSpeedWagon = null;
    [SerializeField] private GameObject roomClearSpeedWagon = null;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        // 뒤로가기 버튼을 눌렀을 때, 정지 및 재개 ★ Time을 통한 실질적인 게임 정지 및 재개
        if (Input.GetKeyDown(KeyCode.Escape)) PauseGame();
    }

    public void PauseGame()
    {
        if (Time.timeScale == 1) Time.timeScale = 0;
        else if (Time.timeScale == 0) Time.timeScale = 1;

        mapPanel.SetActive(false);
        pausePanel.SetActive(!pausePanel.activeSelf);
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

    private IEnumerator NoticeText(string text, float time)
    {
        noticeText.text = text;
        noticeText.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(time);
        noticeText.gameObject.SetActive(false);
    }

    public IEnumerator EnterPortal()
    {
        fadePanel.SetActive(true);
        yield return new WaitForSecondsRealtime(0.2f);

        currentStageID++;
        StageManager.Instace.GenerateStage(currentStageID);
    }

    public IEnumerator StartStage(Vector2 spawnRoom, Dictionary<Vector2, RoomMold> asd)
    {
        roomDictionary = asd;
        currentRoom = roomDictionary[spawnRoom];
        InitialzeMap();
        
        Traveller.Instance.transform.position = new Vector3(currentRoom.x - 2, currentRoom.y + 1, 0) * 100;
        currentRoom.roomData.enabled = true;
        miniMapCamera.transform.position = new Vector3(currentRoom.x - 2, currentRoom.y + 1, -1) * 100;

        yield return new WaitForSecondsRealtime(0.5f);
        fadePanelAnimator.SetTrigger("FadeIn");
        yield return new WaitForSecondsRealtime(0.1f);
        StartCoroutine("StageSpeedWagon");
        yield return new WaitForSecondsRealtime(0.1f);
        fadePanel.SetActive(false);
    }

    public void InitialzeMap()
    {     
        mapGridLayoutGroup.constraintCount = StageManager.Instace.roomLength;
        roomUiArray = new GameObject[StageManager.Instace.roomLength, StageManager.Instace.roomLength];

        for (int i = 0; i < mapGridLayoutGroup.transform.childCount; i++)
        {
            if (i <= StageManager.Instace.roomLength * StageManager.Instace.roomLength)
                mapGridLayoutGroup.transform.GetChild(i).gameObject.SetActive(true);
            else if (i > StageManager.Instace.roomLength * StageManager.Instace.roomLength)
                mapGridLayoutGroup.transform.GetChild(i).gameObject.SetActive(false);

            for (int j = 0; j < mapGridLayoutGroup.transform.GetChild(i).transform.childCount; j++)
                mapGridLayoutGroup.transform.GetChild(i).transform.GetChild(j).gameObject.SetActive(false);
        }

        int c = 0;
        for (int i = StageManager.Instace.roomLength - 1; i >= 0; i--)
        {
            for (int j = 0; j < StageManager.Instace.roomLength; j++)
            {
                roomUiArray[j, i] = mapGridLayoutGroup.transform.GetChild(c).gameObject;
                c++;
            }
        }

        UpdateMap();
    }

    private void UpdateMap()
    {  
        // # 현재 방 > 밝은 회색
        roomUiArray[currentRoom.x, currentRoom.y].transform.Find("CurrentRoom").gameObject.SetActive(true);
        roomUiArray[currentRoom.x, currentRoom.y].transform.Find("StageTheme").gameObject.SetActive(true);
      
        // # 현재 방과 연결된 방 && 들어가지 않았던 방 > 진한 회색
        UpdateRoomUI(0, currentRoom.x, currentRoom.y);
        UpdateRoomUI(1, currentRoom.x, currentRoom.y);
        UpdateRoomUI(2, currentRoom.x, currentRoom.y);
        UpdateRoomUI(3, currentRoom.x, currentRoom.y);
    }

    private void UpdateRoomUI(int doorIndex, int originX, int originY)
    {
        int totalX = originX;
        int totalY = originY;
        string originDoor = "";
        string totalDoor = "";

        if (doorIndex == 0) {totalY++; originDoor = "Up"; totalDoor = "Down";}
        else if (doorIndex == 1) {totalY--; originDoor = "Down"; totalDoor = "Up";}
        else if (doorIndex == 2) {totalX--; originDoor = "Left"; totalDoor = "Right";}
        else if (doorIndex == 3) {totalX++; originDoor = "Right"; totalDoor = "Left";}

        if (currentRoom.isDoorOpen[doorIndex] && !roomDictionary[new Vector2(totalX, totalY)].roomData.isCleared)
        {
            roomUiArray[originX, originY].transform.Find(originDoor).gameObject.SetActive(true);
            roomUiArray[totalX, totalY].transform.Find(totalDoor).gameObject.SetActive(true);

            if (roomDictionary[new Vector2(totalX, totalY)].roomData.roomType == RoomData.RoomType.Boss)
                roomUiArray[totalX, totalY].transform.Find("Boss").gameObject.SetActive(true);
        }
    }
    
    public IEnumerator MigrateRoom(string direction)
    {
        fadePanel.SetActive(true);
        yield return new WaitForSecondsRealtime(0.2f);

        roomUiArray[currentRoom.x, currentRoom.y].transform.Find("CurrentRoom").gameObject.SetActive(false);

        // #_ 이동하는 방향의 방으로 이동
        int totalX = currentRoom.x;
        int totalY = currentRoom.y;
        int totalDoorIndex = 0;
        Vector3 asd = Vector2.zero;

        if (direction == "Up") {totalY++; totalDoorIndex = 1; asd = Vector2.up;}
        else if (direction == "Down") {totalY--; totalDoorIndex = 0; asd = Vector2.down;}
        else if (direction == "Left") {totalX--; totalDoorIndex = 3; asd = Vector2.left;}
        else if (direction == "Right") {totalX++; totalDoorIndex = 2; asd = Vector2.right;}

        currentRoom = roomDictionary[new Vector2(totalX, totalY)];
        Transform totalDoorTransform = currentRoom.roomData.doors[totalDoorIndex].transform;
        Traveller.Instance.transform.position = new Vector3(totalDoorTransform.position.x, totalDoorTransform.position.y, 0) + asd * 2f;
        miniMapCamera.transform.position = new Vector3(currentRoom.roomData.transform.position.x, currentRoom.roomData.transform.position.y, -100);

        UpdateMap();

        if (currentRoom.roomData.isCleared == false)
        {
            bagPanel.SetActive(false);
            mapPanel.SetActive(false);
        }
        
        fadePanelAnimator.SetTrigger("FadeIn");
        currentRoom.roomData.enabled = true;

        yield return new WaitForSecondsRealtime(0.2f);
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
        StageManager.Instace.DestroyStage();

        StopCoroutine("BossSpeedWagon");
        bossSpeedWagon.SetActive(false);
        StopCoroutine("StageSpeedWagon");
        stageSpeedWagon.SetActive(false);
        StopCoroutine("RoomClearSpeedWagon");
        roomClearSpeedWagon.SetActive(false);

        ObjectManager.Instance.InsertAll();
        monsters.Clear();

        Traveller.Instance.enabled = true;
        Traveller.Instance.Initialize();

        miniMapCamera.transform.position = new Vector3(0, 0, -100);

        isFighting = false;
        currentStageID = -1; 
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
        stageNumberText.text = currentStageID.ToString();
        stageNameText.text = StageDataBase.Instance.stages[currentStageID].name;
        yield return new WaitForSeconds(2f);
        stageSpeedWagon.SetActive(false);
    }

    public IEnumerator BossSpeedWagon()
    {
        GameObject.Find("SlimeKing(Clone)").GetComponent<SlimeKing>().enabled = false;
        GameObject.Find("SlimeKing(Clone)").transform.Find("CM Camera1").GetComponent<CinemachineVirtualCamera>().Priority = 100;
        Traveller.Instance.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        bossSpeedWagon.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(3f);
        bossSpeedWagon.gameObject.SetActive(false);
        Traveller.Instance.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        GameObject.Find("SlimeKing(Clone)").transform.Find("CM Camera1").GetComponent<CinemachineVirtualCamera>().Priority = 0;
        GameObject.Find("SlimeKing(Clone)").GetComponent<SlimeKing>().enabled = true;
    }

    public IEnumerator RoomClearSpeedWagon()
    {
        roomClearSpeedWagon.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(2f);
        roomClearSpeedWagon.gameObject.SetActive(false);
    }
}