using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    [SerializeField] private MasteryManager MasteryManager;

    [SerializeField] private BoolVariable isFighting;
    [SerializeField] private BoolVariable isGaming;
    [SerializeField] private BoolVariable isFocusOnSomething;
    [SerializeField] private BoolVariable isLoading;
    public BoolVariable isBossing;
    [SerializeField] private BoolVariable isShowingSomething;

    [SerializeField] private GameEvent onRecall;
    public GameEvent OnRoomClear;
    [SerializeField] private BuffRunTimeSet buffRunTimeSet;

    public EnemyRunTimeSet enemyRunTimeSet;

    [SerializeField] private GameObject gamePanel;
    [SerializeField] private GameObject gameResultPanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject recallButton;
    [SerializeField] private Animator fadePanel;

    [SerializeField] private GameObject undo;
    [SerializeField] private TextMeshProUGUI enemy;

    public CinemachineImpulseSource CinemachineImpulseSource { get; private set; }
    public CinemachineVirtualCamera CinemachineVirtualCamera { get; private set; }
    public CinemachineTargetGroup CinemachineTargetGroup { get; private set; }

    [SerializeField] private IntVariable nyang;
    public IntVariable viewer;

    [SerializeField] private GameObject endingGameObject;
    [SerializeField] private GameObject endingPanel;
    [SerializeField] private Animator endingAnimator;
    private bool clickRecall;
    [SerializeField] protected GameEvent onMonsterCollapse;

    private void Awake()
    {
        Application.targetFrameRate = 60;
        Instance = this;
        nyang.RuntimeValue = 3000;
        CinemachineVirtualCamera = Camera.main.transform.parent.Find("CM Camera").GetComponent<CinemachineVirtualCamera>();
        CinemachineTargetGroup = Camera.main.transform.parent.Find("CM TargetGroup").GetComponent<CinemachineTargetGroup>();
        CinemachineImpulseSource = GetComponent<CinemachineImpulseSource>();
        CinemachineVirtualCamera.m_Lens.OrthographicSize = 12;
    }

    public void Skip()
    {
        onMonsterCollapse.Raise(Wakgood.Instance.transform);
    }

    public void SkipAll()
    {
        int temp = enemyRunTimeSet.Items.Count;
        for (int i = 0; i < temp; i++)
        {
            ObjectManager.Instance.PushObject(enemyRunTimeSet.Items[0]);
            onMonsterCollapse.Raise(Wakgood.Instance.transform);
        }
    }

    private void Start()
    {
        StartCoroutine(CheckBuff());
        UIManager.Instance.SetStageName("마을");
        UIManager.Instance.SetRoomName("마을");
        AudioManager.Instance.PlayMusic("yeppSun - 고고 다섯쌍둥이");
        UIManager.Instance.SetCurViewerText("뱅온 전!");
        CinemachineVirtualCamera.m_Lens.OrthographicSize = 12;
        // enemyRunTimeSet.Clear();
    }

    private IEnumerator CheckBuff()
    {
        while (true)
        {
            foreach (Buff buff in new List<Buff>(buffRunTimeSet.Items))
            {
                if (buff.hasCondition) continue;
                else if (buff.removeTime <= Time.time)
                {
                    buffRunTimeSet.Remove(buff);
                }
            }

            yield return new WaitForSeconds(0.02f);
        }
    }

    private void Update()
    {

        enemy.text = enemyRunTimeSet.Items.Count.ToString();

        isFocusOnSomething.RuntimeValue = (StreamingManager.Instance.IsChatting || isLoading.RuntimeValue || isShowingSomething.RuntimeValue);

        if (Input.GetKeyDown(KeyCode.Escape) && !isLoading.RuntimeValue && !gameResultPanel.activeSelf)
            if (!SettingManager.Instance.Temp())
                if (!StreamingManager.Instance.Temp())
                        PauseGame();                
    }

    public void PauseGame() // 정지 버튼에서 호출
    {
        Time.timeScale = pausePanel.activeSelf ? 1 : 0;

        recallButton.SetActive(isGaming.RuntimeValue);
        pausePanel.SetActive(!pausePanel.activeSelf);
    }

    public IEnumerator EnterPortal(GameData2 gameData2 = null)
    {
        AudioManager.Instance.StopMusic();
        ObjectManager.Instance.DeactivateAll();

        CinemachineVirtualCamera.m_Lens.OrthographicSize = 12;
        isLoading.RuntimeValue = true;
        fadePanel.SetTrigger("OUT");
        yield return new WaitForSeconds(0.2f);
        undo.SetActive(false);

        if (gameData2 != null)
        {
            DataManager dataManager = DataManager.Instance;
            StageManager.Instance.currentStageID = gameData2.lastStageID - 1;

            int temp = gameData2.items.Count;
            for (int i = 0; i < temp; i++)        
                dataManager.wgItemInven.Add(dataManager.ItemDic[gameData2.items[i]]);  

            temp = gameData2.foods.Count;
            for (int i = 0; i < temp; i++)      
                dataManager.wgFoodInven.Add(dataManager.FoodDic[gameData2.foods[i]]);      

            temp = gameData2.masteries.Count;
            for (int i = 0; i < temp; i++)
                dataManager.wgMasteryInven.Add(dataManager.MasteryDic[gameData2.masteries[i]]);

            viewer.RuntimeValue = gameData2.viewer;
            Wakgood.Instance.SwitchWeaponStatic(0, DataManager.Instance.WeaponDic[gameData2.weapon0ID]);
            Wakgood.Instance.SwitchWeaponStatic(1, DataManager.Instance.WeaponDic[gameData2.weapon1ID]);

            Wakgood.Instance.exp.RuntimeValue = gameData2.exp;
            Wakgood.Instance.level.RuntimeValue = gameData2.level;
            Wakgood.Instance.hpCur.RuntimeValue = gameData2.hp;
        }
        StageManager.Instance.GenerateStage();       
    }
    
    public void ClickRecall() => clickRecall = true;

    // OnCollapse GameEvent로 호출
    public void GameOverAndRecall() => Wakgood.Instance.Collapse();

    public IEnumerator _GameOverAndRecall()
    {
        AudioManager.Instance.PlayMusic("위윌왁휴 - 처신 잘 하라고");

        Time.timeScale = 1;
        gamePanel.SetActive(false);
        pausePanel.SetActive(false);
        UIManager.Instance.SetResult();
        gameResultPanel.SetActive(true);

        yield return new WaitForSeconds(1f);

        Wakgood.Instance.gameObject.SetActive(false);

        while (clickRecall == false) yield return null;
        clickRecall = false;

        fadePanel.SetTrigger("OUT");
        yield return new WaitForSeconds(1f);

        CinemachineVirtualCamera.m_Lens.OrthographicSize = 12;
        CinemachineTargetGroup.m_Targets[1].target = null;

        gamePanel.SetActive(true);

        onRecall.Raise();
        isGaming.RuntimeValue = false;
        isFighting.RuntimeValue = false;
        isBossing.RuntimeValue = false;

        StageManager.Instance.DestroyStage();
        UIManager.Instance.SetStageName("마을");
        UIManager.Instance.SetRoomName("마을");
        UIManager.Instance.SetCurViewerText("뱅온 전!");
        AudioManager.Instance.PlayMusic("yeppSun - 고고 다섯쌍둥이");
        StageManager.Instance.currentStageID = -1;

        UIManager.Instance.StopAllSpeedWagons();

        MasteryManager.SetSelectMasteryPanelOff();
        
        enemyRunTimeSet.Clear();
        ObjectManager.Instance.DeactivateAll();

        viewer.RuntimeValue = 10000;

        DataManager.Instance.wgMasteryInven.Clear();
        DataManager.Instance.wgItemInven.Clear();
        DataManager.Instance.wgFoodInven.Clear();
        DataManager.Instance.buffRunTimeSet.Clear();

        nyang.RuntimeValue = 3000;
        viewer.RuntimeValue = 3000;

        gameResultPanel.SetActive(false);
        gamePanel.SetActive(true);

        UIManager.Instance.bossHpBar.HpBarOff();

        Wakgood.Instance.enabled = true;
        Wakgood.Instance.gameObject.SetActive(true);
        undo.SetActive(true);

        yield return new WaitForSeconds(1f);
        fadePanel.SetTrigger("IN");
        Time.timeScale = 1;
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game...");
        DataManager.Instance.SaveGameData();
        Application.Quit();
    }

    public IEnumerator Ending()
    {
        endingGameObject.SetActive(true);
        endingPanel.SetActive(true);

        yield return new WaitForSeconds(.2f);
        yield return new WaitForSeconds(endingAnimator.GetCurrentAnimatorStateInfo(0).length);
        
        endingGameObject.SetActive(false);
        endingPanel.SetActive(false);

        StartCoroutine(Endingg());
    }

    public IEnumerator Endingg()
    {
        AudioManager.Instance.PlayMusic("yeppSun - 왁버거 MR");
        DataManager.Instance.CurGameData.rescuedNPC[29] = true;
        DataManager.Instance.SaveGameData();

        Time.timeScale = 1;
        gamePanel.SetActive(false);
        pausePanel.SetActive(false);
        UIManager.Instance.SetResult(true);
        gameResultPanel.SetActive(true);

        yield return new WaitForSeconds(1f);

        Wakgood.Instance.gameObject.SetActive(false);

        while (clickRecall == false) yield return null;
        clickRecall = false;

        fadePanel.SetTrigger("OUT");
        yield return new WaitForSeconds(1f);

        CinemachineVirtualCamera.m_Lens.OrthographicSize = 12;
        CinemachineTargetGroup.m_Targets[1].target = null;

        gamePanel.SetActive(true);     

        onRecall.Raise();
        isGaming.RuntimeValue = false;
        isFighting.RuntimeValue = false;
        isBossing.RuntimeValue = false;

        StageManager.Instance.DestroyStage();
        UIManager.Instance.SetStageName("마을");
        UIManager.Instance.SetRoomName("마을");
        UIManager.Instance.SetCurViewerText("뱅온 전!");
        AudioManager.Instance.PlayMusic("yeppSun - 고고 다섯쌍둥이");
        StageManager.Instance.currentStageID = -1;

        UIManager.Instance.StopAllSpeedWagons();


        MasteryManager.SetSelectMasteryPanelOff();

        enemyRunTimeSet.Clear();
        ObjectManager.Instance.DeactivateAll();

        viewer.RuntimeValue = 10000;

        DataManager.Instance.wgMasteryInven.Clear();
        DataManager.Instance.wgItemInven.Clear();
        DataManager.Instance.wgFoodInven.Clear();
        DataManager.Instance.buffRunTimeSet.Clear();

        nyang.RuntimeValue = 3000;
        viewer.RuntimeValue = 3000;

        gameResultPanel.SetActive(false);
        gamePanel.SetActive(true);

        UIManager.Instance.bossHpBar.HpBarOff();

        Wakgood.Instance.enabled = true;
        Wakgood.Instance.gameObject.SetActive(true);
        undo.SetActive(true);

        yield return new WaitForSeconds(1f);
        fadePanel.SetTrigger("IN");
        Time.timeScale = 1;
    }

    public Transform GetNearestMob(Transform originTransform)
    {
        Transform target = null;
        float targetDist = 100;
        float currentDist;

        foreach (GameObject monster in enemyRunTimeSet.Items)
        {
            currentDist = Vector2.Distance(originTransform.position, monster.transform.position);
            if (currentDist > targetDist) continue;

            target = monster.transform;
            targetDist = currentDist;
        }

        return target;
    }
}

public class DebugManager
{
    public static void GetItem(int id) => DataManager.Instance.wgItemInven.Add(DataManager.Instance.ItemDic[id]);
    public static void GetWeapon(int id) => Wakgood.Instance.SwitchWeapon(Wakgood.Instance.CurWeaponNumber, DataManager.Instance.WeaponDic[id]);
}