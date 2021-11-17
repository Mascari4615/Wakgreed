using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD.Studio;
using FMODUnity;
using UnityEngine.Serialization;

public class GameManager : MonoBehaviour
{
    [HideInInspector] public static GameManager Instance { get; private set; }

    [SerializeField] private BoolVariable isFighting;
    [SerializeField] private BoolVariable isGaming;
    [SerializeField] private BoolVariable isFocusOnSomething;
    [SerializeField] private BoolVariable isLoading;
    [SerializeField] private BoolVariable isChatting;

    [SerializeField] private GameEvent onRecall;
    [SerializeField] private BuffRunTimeSet buffRunTimeSet;

    public EnemyRunTimeSet enemyRunTimeSet;

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
        testNpc.SetActive(DataManager.Instance.curGameData.isNpcRescued);

        Instance = this;
        StartCoroutine(CheckBuff());
        AudioManager.Instance.BgmEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        AudioManager.Instance.BgmEvent = RuntimeManager.CreateInstance($"event:/BGM/Undo");
        AudioManager.Instance.BgmEvent.start();
    }

    private IEnumerator CheckBuff()
    {
        while (true)
        {
            foreach (Buff buff in new List<Buff>(buffRunTimeSet.Items))
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
        Debug.Log($"{isFocusOnSomething.RuntimeValue}, {isChatting.RuntimeValue}, {isLoading.RuntimeValue}");
        isFocusOnSomething.RuntimeValue = (isChatting.RuntimeValue || isLoading.RuntimeValue);
        
        if (!Input.GetKeyDown(KeyCode.Escape)) return;

        if (StreamingManager.Instance.inputField.gameObject.activeSelf)
        {
            StreamingManager.Instance.inputField.text = "";
            StreamingManager.Instance.inputField.gameObject.SetActive(false);
            StreamingManager.Instance.t = 5;
        }
        else PauseGame();
    }

    public void PauseGame() // 정지 버튼에서 호출
    {
        Time.timeScale = Time.timeScale switch
        {
            1 => 0,
            0 => 1,
            _ => Time.timeScale
        };

        pausePanel.SetActive(!pausePanel.activeSelf);
    }

    public IEnumerator EnterPortal()
    {
        isLoading.RuntimeValue = true;
        fadePanel.SetActive(true);
        yield return new WaitForSeconds(0.2f);

        undo.SetActive(false);
        StageManager.Instance.GenerateStage();
        isGaming.RuntimeValue = true;
    }

    // OnCollapse GameEvent로 호출
    public void GameOver()
    {
        gamePanel.SetActive(false);
        gameResultPanel.SetActive(true);
    }

    public void Recall()
    {
        onRecall.Raise();
        isGaming.RuntimeValue = false;
        isFighting.RuntimeValue = false;

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

        enemyRunTimeSet.Clear();

        ObjectManager.Instance.DeactivateAll();

        DataManager.Instance.wakgoodItemInventory.Clear();
        DataManager.Instance.wakgoodFoodInventory.Clear();
        DataManager.Instance.wakgoodMasteryInventory.Clear();
        DataManager.Instance.buffRunTimeSet.Clear();   

        miniMapCamera.transform.position = new Vector3(0, 0, -100);

        pausePanel.SetActive(false);
        gameResultPanel.SetActive(false);
        gamePanel.SetActive(true);

        testNpc.SetActive(DataManager.Instance.curGameData.isNpcRescued);

        Wakgood.Instance.StopAllCoroutines();
        Wakgood.Instance.enabled = true;
        Wakgood.Instance.Initialize();

        nyang.RuntimeValue = 0;
        viewer.RuntimeValue = 0;

        AudioManager.Instance.BgmEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        AudioManager.Instance.BgmEvent = RuntimeManager.CreateInstance($"event:/BGM/Undo");
        AudioManager.Instance.BgmEvent.start();

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

public class DebugManager
{
    public static void GetItem(int id)
    {
        DataManager.Instance.wakgoodItemInventory.Add(DataManager.Instance.ItemDic[id]);
    }

    public static void GetWeapon(int id)
    {
        Wakgood.Instance.SwitchWeapon(DataManager.Instance.WeaponDic[id]);
    }
}