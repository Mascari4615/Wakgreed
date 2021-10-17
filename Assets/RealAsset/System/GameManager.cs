using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    [HideInInspector] public static GameManager Instance { get { return instance; } }

    [SerializeField] private BoolVariable IsFighting;
    [SerializeField] private BoolVariable IsGaming;
    [SerializeField] private GameEvent OnRecall;
    [SerializeField] private BuffRunTimeSet buffRunTimeSet;

    public EnemyRunTimeSet EnemyRunTimeSet;

    [SerializeField] private GameObject gamePanel;
    [SerializeField] private GameObject miniMapCamera;

    [SerializeField] private GameObject gameResultPanel;

    [SerializeField] private GameObject pausePanel;

    [SerializeField] private GameObject fadePanel;

    [SerializeField] private GameObject roomClearSpeedWagon;
    [SerializeField] private GameObject undo;
    [SerializeField] private GameObject testNpc;

    public CinemachineImpulseSource cinemachineImpulseSource;

    [SerializeField] private IntVariable nyang;
    [SerializeField] private IntVariable viewer;

    private void Awake()
    {
        Application.targetFrameRate = 60;
        testNpc.SetActive(DataManager.Instance.curGameData.isNPCRescued);

        instance = this;
        StartCoroutine(CheckBuff());
        AudioManager.Instance.BGMEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        AudioManager.Instance.BGMEvent = RuntimeManager.CreateInstance($"event:/BGM/Undo");
        AudioManager.Instance.BGMEvent.start();
    }

    private IEnumerator CheckBuff()
    {
        while (true)
        {
            foreach (var buff in new List<Buff>(buffRunTimeSet.Items))
            {
                //Debug.Log($"{name} : CheckBuff - {buff.name}");
                if (buff.hasCondition) continue;
                else if (buff.removeTime <= Time.time)
                {
                    buffRunTimeSet.Remove(buff);
                    //Debug.Log($"{name} : CheckBuff, BuffRemove - {buff.name}");
                }
            }

            yield return new WaitForSeconds(0.02f);
        }
    }

    private void Update()
    {
        // 뒤로가기 버튼을 눌렀을 때, 정지 및 재개 ★ Time을 통한 실질적인 게임 정지 및 재개
        // # 정지 > 귀환 (StopCoroutine) 오류 가능성 : 따라서 플래그를 통해 현재 정지 시킬 수 있는지 확인해야함
        if (Input.GetKeyDown(KeyCode.Escape)) 
        {
            if (SettingManager.Instance.settingPanel.activeSelf) SettingManager.Instance.settingPanel.SetActive(false);
            else PauseGame(); 
        }
    }

    public void PauseGame() // 정지 버튼에서 호출
    {
        if (Time.timeScale == 1) Time.timeScale = 0;
        else if (Time.timeScale == 0) Time.timeScale = 1;

        pausePanel.SetActive(!pausePanel.activeSelf);
    }

    public IEnumerator EnterPortal()
    {
        fadePanel.SetActive(true);
        yield return new WaitForSeconds(0.2f);

        undo.SetActive(false);
        StageManager.Instance.GenerateStage();
        IsGaming.RuntimeValue = true;
    }

    // OnCollapse GameEvent로 호출
    public void GameOver()
    {
        gamePanel.SetActive(false);
        gameResultPanel.SetActive(true);
    }

    public void Recall()
    {
        OnRecall.Raise();
        IsGaming.RuntimeValue = false;
        IsFighting.RuntimeValue = false;

        StageManager.Instance.DestroyStage();
        undo.SetActive(true);
        StageManager.Instance.currentStageID = -1;

        StopAllSpeedWagons();
        StageManager.Instance.StopAllSpeedWagons();

        MasteryManager.Instance.selectMasteryPanel.SetActive(false);

        // UpdateMap();

        /*
        int count = EnemyRunTimeSet.Items.Count;
        for (int i = 0; i < count; i++)
        {
            EnemyRunTimeSet.Items[0].SetActive(false);
            EnemyRunTimeSet.Remove(EnemyRunTimeSet.Items[0]);
        }
        */

        EnemyRunTimeSet.Clear();

        ObjectManager.Instance.DeactiveAll();

        DataManager.Instance.WakgoodItemInventory.Clear();
        DataManager.Instance.WakgoodFoodInventory.Clear();
        DataManager.Instance.WakgoodMasteryInventory.Clear();
        DataManager.Instance.BuffRunTimeSet.Clear();   

        miniMapCamera.transform.position = new Vector3(0, 0, -100);

        pausePanel.SetActive(false);
        gameResultPanel.SetActive(false);
        gamePanel.SetActive(true);

        testNpc.SetActive(DataManager.Instance.curGameData.isNPCRescued);

        Wakgood.Instance.StopAllCoroutines();
        Wakgood.Instance.enabled = true;
        Wakgood.Instance.Initialize();

        nyang.RuntimeValue = 0;
        viewer.RuntimeValue = 0;

        AudioManager.Instance.BGMEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        AudioManager.Instance.BGMEvent = RuntimeManager.CreateInstance($"event:/BGM/Undo");
        AudioManager.Instance.BGMEvent.start();

        Time.timeScale = 1;
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game...");
        DataManager.Instance.SaveGameData();
        Application.Quit();
    }

    public IEnumerator RoomClearSpeedWagon()
    {
        roomClearSpeedWagon.SetActive(true);
        yield return new WaitForSeconds(2f);
        roomClearSpeedWagon.SetActive(false);
    }

    private void StopAllSpeedWagons()
    {
        StopCoroutine(nameof(RoomClearSpeedWagon));
        roomClearSpeedWagon.SetActive(false);
    }
}