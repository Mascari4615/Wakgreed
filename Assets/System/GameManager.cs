﻿using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private BoolVariable isFighting;
    [SerializeField] private BoolVariable isGaming;
    [SerializeField] private BoolVariable isFocusOnSomething;
    [SerializeField] private BoolVariable isLoading;
    public BoolVariable isBossing;
    [SerializeField] private BoolVariable isShowingSomething;

    [SerializeField] private GameEvent onRecall;
    [SerializeField] private BuffRunTimeSet buffRunTimeSet;

    public EnemyRunTimeSet enemyRunTimeSet;

    [SerializeField] private GameObject gamePanel;
    [SerializeField] private GameObject gameResultPanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private GameObject recallButton;
    [SerializeField] private Animator fadePanel;

    [SerializeField] private GameObject undo;

    public CinemachineImpulseSource CinemachineImpulseSource { get; private set; }
    public CinemachineVirtualCamera CinemachineVirtualCamera { get; private set; }
    public CinemachineTargetGroup CinemachineTargetGroup { get; private set; }

    [SerializeField] private IntVariable nyang;
    [SerializeField] private IntVariable viewer;

    [SerializeField] private GameObject endingGameObject;
    [SerializeField] private GameObject endingPanel;
    [SerializeField] private Animator endingAnimator;
    private bool clickRecall;

    private void Awake()
    {
        Application.targetFrameRate = 60;
        Instance = this;

        CinemachineVirtualCamera = Camera.main.transform.parent.Find("CM Camera").GetComponent<CinemachineVirtualCamera>();
        CinemachineTargetGroup = Camera.main.transform.parent.Find("CM TargetGroup").GetComponent<CinemachineTargetGroup>();
        CinemachineImpulseSource = GetComponent<CinemachineImpulseSource>();
        CinemachineVirtualCamera.m_Lens.OrthographicSize = 12;
    }

    private void Start()
    {
        StartCoroutine(CheckBuff());
        UIManager.Instance.SetStageName("마을");
        AudioManager.Instance.PlayMusic("yeppSun - 고고 다섯쌍둥이");
        UIManager.Instance.SetCurViewerText("뱅온 전!");
        CinemachineVirtualCamera.m_Lens.OrthographicSize = 12;
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

    public IEnumerator EnterPortal()
    {
        AudioManager.Instance.StopMusic();
        ObjectManager.Instance.DeactivateAll();

        CinemachineVirtualCamera.m_Lens.OrthographicSize = 12;
        isLoading.RuntimeValue = true;
        fadePanel.SetTrigger("OUT");
        yield return new WaitForSeconds(0.2f);

        undo.SetActive(false);
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

        gamePanel.SetActive(true);

        onRecall.Raise();
        isGaming.RuntimeValue = false;
        isFighting.RuntimeValue = false;
        isBossing.RuntimeValue = false;

        StageManager.Instance.DestroyStage();
        UIManager.Instance.SetStageName("마을");
        AudioManager.Instance.PlayMusic("yeppSun - 고고 다섯쌍둥이");
        StageManager.Instance.currentStageID = -1;

        UIManager.Instance.StopAllSpeedWagons();

        MasteryManager.Instance.selectMasteryPanel.SetActive(false);
        
        enemyRunTimeSet.Clear();
        ObjectManager.Instance.DeactivateAll();

        viewer.RuntimeValue = 10000;

        DataManager.Instance.wakgoodMasteryInventory.Clear();
        DataManager.Instance.wakgoodItemInventory.Clear();
        DataManager.Instance.wakgoodFoodInventory.Clear();
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

        gamePanel.SetActive(true);

        nyang.RuntimeValue = 3000;
        viewer.RuntimeValue = 10000;

        onRecall.Raise();
        isGaming.RuntimeValue = false;
        isFighting.RuntimeValue = false;
        isBossing.RuntimeValue = false;

        StageManager.Instance.DestroyStage();
        UIManager.Instance.SetStageName("마을");
        AudioManager.Instance.PlayMusic("yeppSun - 고고 다섯쌍둥이");
        StageManager.Instance.currentStageID = -1;

        UIManager.Instance.StopAllSpeedWagons();

        MasteryManager.Instance.selectMasteryPanel.SetActive(false);

        enemyRunTimeSet.Clear();
        ObjectManager.Instance.DeactivateAll();

        DataManager.Instance.wakgoodMasteryInventory.Clear();
        DataManager.Instance.wakgoodItemInventory.Clear();
        DataManager.Instance.wakgoodFoodInventory.Clear();
        DataManager.Instance.buffRunTimeSet.Clear();

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
}

public class DebugManager
{
    public static void GetItem(int id) => DataManager.Instance.wakgoodItemInventory.Add(DataManager.Instance.ItemDic[id]);
    public static void GetWeapon(int id) => Wakgood.Instance.SwitchWeapon(Wakgood.Instance.CurWeaponNumber, DataManager.Instance.WeaponDic[id]);
}