using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    [HideInInspector] public static GameManager Instance { get { return instance; } }

    [HideInInspector] public int nyang = 0;
    [HideInInspector] public int monsterKill = 0;
    [HideInInspector] public bool isStageCleared = false;
    [HideInInspector] public bool isFighting = false;
    private int currentStageNumber = -1;

    private Dictionary<int[], RoomMold> roomDictionary = new Dictionary<int[], RoomMold>();
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
    
    private IEnumerator coroutine;
    [SerializeField] private GameObject monsterPool;

    void Awake()
    {
        instance = this;
        coroutine = NoticeText("전투 중에는 열 수 없습니다.", 1.5f);
        roomDictionary = StageManager.Instace.roomDictionary;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) // 뒤로가기 버튼을 눌렀을 때, 정지 및 재개 ★ Time을 통한 실질적인 게임 정지 및 재개
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        if (pausePanel.activeSelf == true) 
        {
            Time.timeScale = 1;
            pausePanel.SetActive(false); // 뒤로가기 버튼을 눌렀을 때, 정지Panel이 @활성화 돼있다면, 일시정지
        }
        else if (pausePanel.activeSelf == false) 
        {
            Time.timeScale = 0;
            mapPanel.SetActive(false);
            pausePanel.SetActive(true);
        } // 뒤로가기 버튼을 눌렀을 때, 정지Panel이 @비활성화 돼있다면, 재개
    }

    public void OpenAndCloseBag()
    {
        if (isFighting)
        {
            StopCoroutine(coroutine);
            coroutine = NoticeText("전투 중에는 열 수 없습니다.", 1.5f);
            StartCoroutine(coroutine);
        }
        else
        {
            StopCoroutine(coroutine);

            if (bagPanel.activeSelf == true)
            {
                bagPanel.SetActive(false);
            }
            else if (bagPanel.activeSelf == false)
            {
                bagPanel.SetActive(true);
            }
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

        // player.gameObject.SetActive(false);

        isStageCleared = false;
        currentStageNumber ++;
        StageManager.Instace.GenerateStage(currentStageNumber);
    }

    public IEnumerator StartStage(int[] spawnRoom)
    {
        currentRoom = roomDictionary[spawnRoom];
        InitialzeMap();
        
        Traveller.Instance.transform.position = new Vector3(currentRoom.roomData.transform.position.x, currentRoom.roomData.transform.position.y, 0);
        currentRoom.roomData.enabled = true;
        // player.gameObject.SetActive(true);
        miniMapCamera.transform.position = new Vector3(currentRoom.roomData.transform.position.x, currentRoom.roomData.transform.position.y, -100);

        yield return new WaitForSecondsRealtime(0.5f);
        fadePanelAnimator.SetTrigger("FadeIn");
        yield return new WaitForSecondsRealtime(0.1f);
        StartCoroutine(SpeedWagonManager.Instance.StageSpeedWagon(currentStageNumber));
        yield return new WaitForSecondsRealtime(0.1f);
        fadePanel.SetActive(false);
    }

    public void InitialzeMap()
    {     
        mapGridLayoutGroup.constraintCount = StageManager.Instace.roomMoldLength;

        roomUiArray = null;
        roomUiArray = new GameObject[StageManager.Instace.roomMoldLength, StageManager.Instace.roomMoldLength];

        int c = 0;
        for (int i = StageManager.Instace.roomMoldLength - 1; i >= 0; i--)
        {
            for (int j = 0; j < StageManager.Instace.roomMoldLength; j++)
            {
                roomUiArray[j, i] = mapGridLayoutGroup.transform.GetChild(c).gameObject;
                for (int k = 0; k < mapGridLayoutGroup.transform.GetChild(c).transform.childCount; k++)
                {           
                    mapGridLayoutGroup.transform.GetChild(c).transform.GetChild(k).gameObject.SetActive(false);
                }
                c++;
            }
        }

        UpdateMap();
    }

    private void UpdateMap()
    {  
        int x = currentRoom.x;
        int y = currentRoom.y;

        // # 현재 방 > 밝은 회색
        roomUiArray[currentRoom.x, currentRoom.y].transform.Find("CurrentRoom").gameObject.SetActive(true);
        roomUiArray[currentRoom.x, currentRoom.y].transform.Find("1").gameObject.SetActive(true);
        roomUiArray[currentRoom.x, currentRoom.y].transform.Find("2").gameObject.SetActive(true);
      
        // # 현재 방과 연결된 방 && 들어가지 않았던 방 > 진한 회색
        if (currentRoom.isDoorOpen[0] && !roomDictionary[new int[]{x, y + 1}].roomData.isCleared)
        {
            roomUiArray[x, y].transform.Find("Up").gameObject.SetActive(true);
            roomUiArray[x, y + 1].transform.Find("Down").gameObject.SetActive(true);

            if (roomDictionary[new int[]{x, y + 1}].roomData.roomType == RoomData.RoomType.Boss)
            {
                roomUiArray[x, y + 1].transform.Find("Boss").gameObject.SetActive(true);
            }
        }

        if (currentRoom.isDoorOpen[1] && !roomDictionary[new int[]{x, y - 1}].roomData.isCleared)
        {
            roomUiArray[x, y].transform.Find("Down").gameObject.SetActive(true);
            roomUiArray[x, y - 1].transform.Find("Up").gameObject.SetActive(true);

            if (roomDictionary[new int[]{x, y - 1}].roomData.roomType == RoomData.RoomType.Boss)
            {
                roomUiArray[x, y - 1].transform.Find("Boss").gameObject.SetActive(true);
            }
        }  

        if (currentRoom.isDoorOpen[2] && !roomDictionary[new int[]{x - 1, y}].roomData.isCleared)
        {
            roomUiArray[x, y].transform.Find("Left").gameObject.SetActive(true);
            roomUiArray[x - 1, y].transform.Find("Right").gameObject.SetActive(true);

            if (roomDictionary[new int[]{x - 1, y}].roomData.roomType == RoomData.RoomType.Boss)
            {
                roomUiArray[x - 1, y].transform.Find("Boss").gameObject.SetActive(true);
            }
        }

        if (currentRoom.isDoorOpen[3] && !roomDictionary[new int[]{x + 1, y}].roomData.isCleared)
        {
            roomUiArray[x, y].transform.Find("Right").gameObject.SetActive(true);
            roomUiArray[x + 1, y].transform.Find("Left").gameObject.SetActive(true);

            if (roomDictionary[new int[]{x + 1, y}].roomData.roomType == RoomData.RoomType.Boss)
            {
                roomUiArray[x + 1, y].transform.Find("Boss").gameObject.SetActive(true);
            }
        }
    }
    
    public IEnumerator MigrateRoom(string direction)
    {
        fadePanel.SetActive(true);
        yield return new WaitForSecondsRealtime(0.2f);
        Time.timeScale = 0;

        roomUiArray[currentRoom.x, currentRoom.y].transform.Find("CurrentRoom").gameObject.SetActive(false);

        // #_ 이동하는 방향의 방으로 이동
        switch (direction) 
        {
            case "Up" :
                currentRoom = roomDictionary[new int[]{currentRoom.x, currentRoom.y + 1}];
                Traveller.Instance.transform.position = new Vector3(currentRoom.roomData.doors[1].transform.position.x, currentRoom.roomData.doors[1].transform.position.y, 0) + Vector3.up * 2f;
            break;

            case "Down" :
                currentRoom = roomDictionary[new int[]{currentRoom.x, currentRoom.y - 1}];
                Traveller.Instance.transform.position = new Vector3(currentRoom.roomData.doors[0].transform.position.x, currentRoom.roomData.doors[0].transform.position.y, 0) + Vector3.down * 2f;
            break;

            case "Left" :
                currentRoom = roomDictionary[new int[]{currentRoom.x - 1, currentRoom.y}];
                Traveller.Instance.transform.position = new Vector3(currentRoom.roomData.doors[3].transform.position.x, currentRoom.roomData.doors[3].transform.position.y, 0) + Vector3.left * 2f;
            break;

            case "Right" :
                currentRoom = roomDictionary[new int[]{currentRoom.x + 1, currentRoom.y}];
                Traveller.Instance.transform.position = new Vector3(currentRoom.roomData.doors[2].transform.position.x, currentRoom.roomData.doors[2].transform.position.y, 0) + Vector3.right * 2f;
            break;
            
            default :
                Debug.Log("ERROR : GameManager.MigrateRoom");
            break;
        }

        miniMapCamera.transform.position = new Vector3(currentRoom.roomData.transform.position.x, currentRoom.roomData.transform.position.y, -100);

        UpdateMap();
        
        Time.timeScale = 1;
        fadePanelAnimator.SetTrigger("FadeIn");
        currentRoom.roomData.enabled = true;

        if (currentRoom.roomData.isCleared == false)
        {
            bagPanel.SetActive(false);
            mapPanel.SetActive(false);
        }

        yield return new WaitForSecondsRealtime(0.2f);
        fadePanel.SetActive(false);
    }

    public IEnumerator GameOver()
    {
        Debug.Log("GameOver");
        // 플레이어 스크립트에서 hp <= 0, Died 감지 > 마지막 처리 후 플레이어 스크립트 비활성화
        yield return new WaitForSeconds(2f); // 2초 동안 Player Died/Recall 애니메이션 실행
        Traveller.Instance.gameObject.SetActive(false);

        gamePanel.SetActive(false); // 게임 Panel @비활성화
        gameOverPanel.SetActive(true); // 게임 결과 Panel @활성화
    }
    
    public void Recall()
    {
        StopCoroutine(SpeedWagonManager.Instance.BossSpeedWagon());
        StopCoroutine(SpeedWagonManager.Instance.StageSpeedWagon(currentStageNumber));
        StopCoroutine(SpeedWagonManager.Instance.RoomClearSpeedWagon());

        for (int i = 0; i < monsterPool.transform.childCount; i++)
        {
            monsterPool.transform.GetChild(i).GetComponent<Monster>().InsertQueue();
        }

        Traveller.Instance.enabled = true;
        Traveller.Instance.gameObject.SetActive(true);
        Traveller.Instance.Initialize();

        miniMapCamera.transform.position = new Vector3(0, 0, -100);

        isFighting = false;
        currentStageNumber = -1; 
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
            StopCoroutine(coroutine);
            coroutine = NoticeText("전투 중에는 열 수 없습니다.", 1.5f);
            StartCoroutine(coroutine);
        }
        else
        {
            StopCoroutine(coroutine);

            if (mapPanel.activeSelf == true)
            {
                mapPanel.SetActive(false);
            }
            else if (mapPanel.activeSelf == false)
            {
                // 왼쪽 위 0
                if (mapGridLayoutGroup != null)
                    mapGridLayoutGroup.transform.localPosition = Vector3.zero;
                mapPanel.SetActive(true);
            }
        }    
    }

    public void QuitGame()
    { 
        Debug.Log("Quitting Game...");
        Application.Quit();
    }
}