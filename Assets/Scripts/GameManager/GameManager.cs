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

    public Traveller player = null;
    public ItemBuffer itemBuffer = null;

    [HideInInspector] public int nyang = 0;
    [HideInInspector] public int monsterKill = 0;
    public enum ValueType { Nyang, MonsterKill }

    [HideInInspector] public bool isStageCleared = false;
    [HideInInspector] public bool isFighting = false;

    // 그 외
    public int currentStageNumber = -1;

    // 씬에서 존재하는 오브젝트
    [SerializeField] private GameObject legacyOfTheHero;
    
    // Canvas에서 존재하는 오브젝트
    [SerializeField] private GameObject gamePanel = null;
    public GameObject attackButton = null;
    public GameObject interactionButton = null;
    [SerializeField] private Text monsterKillText = null;
    [SerializeField] private Text nyangText = null;
    [SerializeField] private GameObject stageSpeedWagon = null;
    [SerializeField] private Text stageNumberText, stageNameText = null;
    [SerializeField] private GameObject bossSpeedWagon = null;
    [SerializeField] GameObject roomClearSpeedWagon = null;
    public Image hpBar = null;
    public Text hpText = null;
    public Image expBar = null;
    public Text levelText = null;
    public Text expText = null;
    [SerializeField] private GameObject gameOverPanel = null;   
    [SerializeField] private GameObject pausePanel = null;
    [HideInInspector] public enum InteractionObjext { None, MainPortal, Portal, UpperDoor, LowerDoor, LeftDoor, RightDoor }
    [HideInInspector] public InteractionObjext nearInteractionObject;
    private Dictionary<string, RoomMold> roomDictionary = new Dictionary<string, RoomMold>();
    public RoomMold currentRoom = null;
    [SerializeField] private ScrollRect scrollRect = null;
    [SerializeField] GameObject mapGridLayoutGroupPreFab = null;
    [SerializeField] GridLayoutGroup mapGridLayoutGroup = null;
    [SerializeField] GameObject roomUi = null;
    private GameObject[,] roomUiArray = null;
    public GameObject mapPanel = null;
    [SerializeField] private GameObject fadePanel = null;
    [SerializeField] private Animator fadePanelAnimator = null;
    [SerializeField] private GameObject bagPanel = null;
    [SerializeField] private Text noticeText = null;

    [HideInInspector] public List<GameObject> monsters = null;

    public GameObject levelUpEffect = null;
    public GameObject bloodingPanel = null;

    [SerializeField] private GameObject miniMapCamera = null;
    [SerializeField] private GameObject selectAbilityPanel = null;
    [SerializeField] private GameObject[] selectAbilityButton = null;
    [SerializeField] private AudioSource selectAbilityAudioSource = null;
    private int selectAbilityStack = 0;

    IEnumerator coroutine = null;

    void Awake()
    {
        instance = this;
        coroutine = NoticeText("전투 중에는 열 수 없습니다.", 1.5f);
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

    private IEnumerator EnterPortal()
    {
        fadePanel.SetActive(true);
        yield return new WaitForSecondsRealtime(0.2f);

        player.gameObject.SetActive(false);

        isStageCleared = false;
        currentStageNumber ++;
        StageManager.Instace.GenerateStage(currentStageNumber);
    }

    public IEnumerator StartStage(Dictionary<string, RoomMold> _roomDictionary, string spawnRoomCoordinate)
    {
        roomDictionary = _roomDictionary;
        currentRoom = roomDictionary[spawnRoomCoordinate];
        InitialzeMap();
        
        player.transform.position = new Vector3(currentRoom.roomData.transform.position.x, currentRoom.roomData.transform.position.y, 0);
        currentRoom.roomData.enabled = true;
        player.gameObject.SetActive(true);
        miniMapCamera.transform.position = new Vector3(currentRoom.roomData.transform.position.x, currentRoom.roomData.transform.position.y, -100);

        yield return new WaitForSecondsRealtime(0.5f);
        fadePanelAnimator.SetTrigger("FadeIn");
        yield return new WaitForSecondsRealtime(0.1f);
        StartCoroutine(StageSpeedWagon());
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
        if (currentRoom.isDoorOpen[0] && !roomDictionary[x + "_" + (y + 1)].roomData.isCleared)
        {
            roomUiArray[x, y].transform.Find("Up").gameObject.SetActive(true);
            roomUiArray[x, y + 1].transform.Find("Down").gameObject.SetActive(true);

            if (roomDictionary[x + "_" + (y + 1)].roomData.roomType == RoomData.RoomType.Boss)
            {
                roomUiArray[x, y + 1].transform.Find("Boss").gameObject.SetActive(true);
            }
        }

        if (currentRoom.isDoorOpen[1] && !roomDictionary[x + "_" + (y - 1)].roomData.isCleared)
        {
            roomUiArray[x, y].transform.Find("Down").gameObject.SetActive(true);
            roomUiArray[x, y - 1].transform.Find("Up").gameObject.SetActive(true);

            if (roomDictionary[x + "_" + (y - 1)].roomData.roomType == RoomData.RoomType.Boss)
            {
                roomUiArray[x, y - 1].transform.Find("Boss").gameObject.SetActive(true);
            }
        }  

        if (currentRoom.isDoorOpen[2] && !roomDictionary[(x - 1) + "_" + y].roomData.isCleared)
        {
            roomUiArray[x, y].transform.Find("Left").gameObject.SetActive(true);
            roomUiArray[x - 1, y].transform.Find("Right").gameObject.SetActive(true);

            if (roomDictionary[(x - 1) + "_" + y].roomData.roomType == RoomData.RoomType.Boss)
            {
                roomUiArray[x - 1, y].transform.Find("Boss").gameObject.SetActive(true);
            }
        }

        if (currentRoom.isDoorOpen[3] && !roomDictionary[(x + 1) + "_" + y].roomData.isCleared)
        {
            roomUiArray[x, y].transform.Find("Right").gameObject.SetActive(true);
            roomUiArray[x + 1, y].transform.Find("Left").gameObject.SetActive(true);

            if (roomDictionary[(x + 1) + "_" + y].roomData.roomType == RoomData.RoomType.Boss)
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
                currentRoom = roomDictionary[currentRoom.x + "_" + (currentRoom.y + 1)];
                player.transform.position = new Vector3(currentRoom.roomData.doors[1].transform.position.x, currentRoom.roomData.doors[1].transform.position.y, 0) + Vector3.up * 2f;
            break;

            case "Down" :
                currentRoom = roomDictionary[currentRoom.x + "_" + (currentRoom.y - 1)];
                player.transform.position = new Vector3(currentRoom.roomData.doors[0].transform.position.x, currentRoom.roomData.doors[0].transform.position.y, 0) + Vector3.down * 2f;
            break;

            case "Left" :
                currentRoom = roomDictionary[(currentRoom.x - 1) + "_" + currentRoom.y];
                player.transform.position = new Vector3(currentRoom.roomData.doors[3].transform.position.x, currentRoom.roomData.doors[3].transform.position.y, 0) + Vector3.left * 2f;
            break;

            case "Right" :
                currentRoom = roomDictionary[(currentRoom.x + 1) + "_" + currentRoom.y];
                player.transform.position = new Vector3(currentRoom.roomData.doors[2].transform.position.x, currentRoom.roomData.doors[2].transform.position.y, 0) + Vector3.right * 2f;
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

    private IEnumerator StageSpeedWagon()
    {
        stageSpeedWagon.SetActive(true);
        stageNumberText.text = currentStageNumber + "";
        stageNameText.text = StageManager.Instace.stageData.stageName;
        yield return new WaitForSeconds(2f);
        stageSpeedWagon.SetActive(false);
    }

    public IEnumerator BossSpeedWagon()
    {
        GameObject.Find("SlimeKing(Clone)").GetComponent<SlimeKing>().enabled = false;
        GameObject.Find("SlimeKing(Clone)").transform.Find("CM Camera1").GetComponent<CinemachineVirtualCamera>().Priority = 100;
        gamePanel.SetActive(false);
        player.playerRB.bodyType = RigidbodyType2D.Static;
        player.enabled = false;
        bossSpeedWagon.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(3f);
        bossSpeedWagon.gameObject.SetActive(false);
        player.enabled = true;
        player.playerRB.bodyType = RigidbodyType2D.Dynamic;
        gamePanel.SetActive(true);
        GameObject.Find("SlimeKing(Clone)").transform.Find("CM Camera1").GetComponent<CinemachineVirtualCamera>().Priority = 0;
        GameObject.Find("SlimeKing(Clone)").GetComponent<SlimeKing>().enabled = true;
    }

    public IEnumerator RoomClearSpeedWagon()
    {
        roomClearSpeedWagon.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(2f);
        roomClearSpeedWagon.gameObject.SetActive(false);
    }

    public IEnumerator GameOver()
    {
        Debug.Log("GameOver");
        // 플레이어 스크립트에서 hp <= 0, Died 감지 > 마지막 처리 후 플레이어 스크립트 비활성화
        yield return new WaitForSeconds(2f); // 2초 동안 Player Died/Recall 애니메이션 실행
        player.gameObject.SetActive(false);

        Debug.Log("* Don't lose hope.");
        Instantiate(legacyOfTheHero, player.gameObject.transform.position, Quaternion.identity);

        gamePanel.SetActive(false); // 게임 Panel @비활성화
        gameOverPanel.SetActive(true); // 게임 결과 Panel @활성화
    }
    
    public void Recall()
    {
        for (int i = 0; i < transform.GetChild(0).childCount; i++)
        {
            transform.GetChild(0).GetChild(i).GetComponent<Monster>().InsertQueue();
        }

        player.gameObject.transform.position = Vector3.zero;
        player.enabled = true;
        player.gameObject.SetActive(true);
        player.Initialize();

        miniMapCamera.transform.position = new Vector3(0, 0, -100);

        isFighting = false;
        currentStageNumber = -1; 
        gameOverPanel.SetActive(false);
        gamePanel.SetActive(true);
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

    public void Interaction()
    {
        interactionButton.SetActive(false);

        if (nearInteractionObject == InteractionObjext.Portal)
        {
            StartCoroutine(EnterPortal());
        }
    }

    public void QuitGame()
    { 
        Debug.Log("Quitting Game...");
        Application.Quit();
    }

    public void LevelUp()
    {
        ObjectManager.Instance.GetQueue(PoolType.Smoke, player.gameObject.transform);
        selectAbilityStack++;
        selectAbilityPanel.SetActive(true);

        // abilityButton[]
    }

    public void ChooseAbility(int i)
    {
        selectAbilityAudioSource.Play();
        selectAbilityStack--;

        switch (i)
        {
            case 0 :
                player.ad += 10;
            break;
            case 1 :
                player.ad += 10;
            break;
            case 2 :
                player.ad += 10;
            break;
            default :
                Debug.Log("???");
            break;
        }

        if (selectAbilityStack > 0)
        {
            selectAbilityPanel.SetActive(true);
        }
        else
        {
            selectAbilityPanel.SetActive(false);
        }  
    }
}