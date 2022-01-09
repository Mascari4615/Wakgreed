using Cinemachine;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum AreaType
{
    Normal,
    Restaurant,
    Shop
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private BoolVariable isFighting;
    [SerializeField] private BoolVariable isGaming;
    [SerializeField] private BoolVariable isFocusOnSomething;
    [SerializeField] private BoolVariable isLoading;
    public BoolVariable isBossing;
    public BoolVariable isRealBossing;
    public bool isRealBossFirstDeath = true;
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

    [SerializeField] private CanvasGroup hosting;
    [SerializeField] private TextMeshProUGUI text1;
    [SerializeField] private TextMeshProUGUI text2;
    [SerializeField] private TextMeshProUGUI text3;

    public CinemachineImpulseSource CinemachineImpulseSource { get; private set; }
    public CinemachineVirtualCamera CinemachineVirtualCamera { get; private set; }
    public CinemachineTargetGroup CinemachineTargetGroup { get; private set; }

    [SerializeField] private IntVariable Goldu;
    public IntVariable viewer;

    [SerializeField] private GameObject endingGameObject;
    [SerializeField] private GameObject endingPanel;
    [SerializeField] private Animator endingAnimator;
    private bool clickRecall;
    [SerializeField] protected GameEvent onMonsterCollapse;

    public AreaType curArea = AreaType.Normal;
    public AreaDoor curAreaDoor = null;

    private const float 카메라사이즈 = 12;
    private const int 최대소지골두 = 5000;

    public void ChangeArea(Transform areaDoor)
    {
        curAreaDoor = areaDoor.GetComponent<AreaDoor>();
        curArea = curAreaDoor.targetAreaType;
        curAreaDoor.originalAreaObject.SetActive(false);
        curAreaDoor.targetAreaObject.SetActive(true);
    }

    public void SwitchCurAreaToNormal()
    {
        curArea = AreaType.Normal;
        curAreaDoor.originalAreaObject.SetActive(true);
        curAreaDoor.targetAreaObject.SetActive(false);
    }

    private void Awake()
    {
        Application.targetFrameRate = 60;
        Instance = this;
        CinemachineVirtualCamera = Camera.main.transform.parent.Find("CM Camera").GetComponent<CinemachineVirtualCamera>();
        CinemachineTargetGroup = Camera.main.transform.parent.Find("CM TargetGroup").GetComponent<CinemachineTargetGroup>();
        CinemachineImpulseSource = GetComponent<CinemachineImpulseSource>();
        CinemachineVirtualCamera.m_Lens.OrthographicSize = 카메라사이즈;
    }

    private void Start()
    {
        StartCoroutine(CheckBuff());
        AudioManager.Instance.PlayMusic("yeppSun - 고고 다섯쌍둥이");
        CinemachineVirtualCamera.m_Lens.OrthographicSize = 카메라사이즈;
        Goldu.RuntimeValue = DataManager.Instance.CurGameData.goldu;
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

        CinemachineVirtualCamera.m_Lens.OrthographicSize = 카메라사이즈;
        isLoading.RuntimeValue = true;
        fadePanel.SetTrigger("OUT");
        yield return new WaitForSeconds(0.2f);
        undo.SetActive(false);

        StageManager.Instance.GenerateStage();
    }

    public void ClickRecall() => clickRecall = true;

    // OnCollapse GameEvent로 호출
    public void GameOverAndRecall() => Wakgood.Instance.Collapse();

    public IEnumerator _GameOverAndRecall(bool isEnding = false)
    {
        if (isEnding)
        {
            AudioManager.Instance.PlayMusic("yeppSun - 왁버거 MR");
            DataManager.Instance.CurGameData.rescuedNPC[29] = true;
            DataManager.Instance.SaveGameData();
        }
        else
        {
            AudioManager.Instance.PlayMusic("위윌왁휴 - 처신 잘 하라고");
        }

        Time.timeScale = 1;
        gamePanel.SetActive(false);
        pausePanel.SetActive(false);
        UIManager.Instance.SetResult(Goldu.RuntimeValue, isEnding);
        gameResultPanel.SetActive(true);

        yield return new WaitForSeconds(1f);

        Wakgood.Instance.gameObject.SetActive(false);

        while (clickRecall == false) yield return null;
        clickRecall = false;

        fadePanel.SetTrigger("OUT");
        yield return new WaitForSeconds(.4f);

        gameResultPanel.SetActive(false);
        gamePanel.SetActive(true);

        CinemachineVirtualCamera.m_Lens.OrthographicSize = 카메라사이즈;
        CinemachineTargetGroup.m_Targets[1].target = null;

        isGaming.RuntimeValue = false;
        isFighting.RuntimeValue = false;
        isBossing.RuntimeValue = false;
        isRealBossing.RuntimeValue = false;
        isRealBossFirstDeath = true;
        StreamingManager.Instance.temp = false;

        StreamingManager.Instance.donationUI[0].SetActive(false);
        StreamingManager.Instance.donationUI[1].SetActive(false);
        StreamingManager.Instance.donationUI[2].SetActive(false);
        StreamingManager.Instance.donationUI[3].SetActive(false);
        StreamingManager.Instance.donationUI[4].SetActive(false);

        StageManager.Instance.DestroyStage();
        UIManager.Instance.SetStageName("마을");
        UIManager.Instance.SetRoomName("마을");
        UIManager.Instance.SetCurViewerText("뱅온 전!");
        AudioManager.Instance.PlayMusic("yeppSun - 고고 다섯쌍둥이");
        StageManager.Instance.currentStageID = -1;

        UIManager.Instance.StopAllSpeedWagons();

        enemyRunTimeSet.Clear();
        ObjectManager.Instance.DeactivateAll();

        viewer.RuntimeValue = 100000;
        DataManager.Instance.wgItemInven.Clear();
        DataManager.Instance.wgFoodInven.Clear();
        DataManager.Instance.buffRunTimeSet.Clear();

        DataManager.Instance.CurGameData.level = Wakgood.Instance.level.RuntimeValue;
        DataManager.Instance.CurGameData.exp = Wakgood.Instance.exp.RuntimeValue;
        viewer.RuntimeValue = 3000;
        Goldu.RuntimeValue = Mathf.Clamp(Goldu.RuntimeValue, 0, 최대소지골두);
        DataManager.Instance.CurGameData.goldu = Goldu.RuntimeValue;
        DataManager.Instance.SaveGameData();

        UIManager.Instance.BossHpBarOff();

        Wakgood.Instance.enabled = true;
        Wakgood.Instance.gameObject.SetActive(true);
        Wakgood.Instance.transform.position = Vector3.zero;
        onRecall.Raise();
        undo.SetActive(true);

        yield return new WaitForSeconds(.4f);
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

        int temp = enemyRunTimeSet.Items.Count;
        for (int i = 0; i < temp; i++)
        {
            ObjectManager.Instance.PushObject(enemyRunTimeSet.Items[0]);
            onMonsterCollapse.Raise(Wakgood.Instance.transform);
        }

        yield return new WaitForSeconds(.2f);
        yield return new WaitForSeconds(endingAnimator.GetCurrentAnimatorStateInfo(0).length);

        endingGameObject.SetActive(false);
        endingPanel.SetActive(false);

        StartCoroutine(_GameOverAndRecall(true));
    }

    public IEnumerator FakeEnding()
    {
        AudioManager.Instance.PlayMusic("위윌왁휴 - 처신 잘 하라고");

        Time.timeScale = 1;
        gamePanel.SetActive(false);
        pausePanel.SetActive(false);
        UIManager.Instance.SetResult(Goldu.RuntimeValue);
        gameResultPanel.SetActive(true);

        yield return new WaitForSeconds(1f);

        while (clickRecall == false) yield return null;
        clickRecall = false;
        AudioManager.Instance.StopMusic();

        gameResultPanel.SetActive(false);
        gamePanel.SetActive(true);

        yield return new WaitForSeconds(.5f);

        viewer.RuntimeValue = 3000;
        ObjectManager.Instance.PopObject("AnimatedText", transform.position + Vector3.up).GetComponent<AnimatedText>().SetText($"시청자 +{3000}", Color.white);
        StreamingManager.Instance.donationUI[1].SetActive(false);
        StreamingManager.Instance.donationText[1].text = $"고니잠님이 호스팅하였습니다";
        StreamingManager.Instance.donationImageUI[1].sprite = StreamingManager.Instance.donationImages[3];
        StreamingManager.Instance.donationUI[1].SetActive(true);
        RuntimeManager.PlayOneShot($"event:/SFX/ETC/Donation");
        yield return new WaitForSeconds(3f);

        hosting.alpha = 0;
        hosting.gameObject.SetActive(true);
        text1.gameObject.SetActive(false);
        text2.gameObject.SetActive(false);

        for (float i = 0; i < 1; i += Time.deltaTime / 3)
        {
            hosting.alpha = i;
        }

        yield return new WaitForSeconds(1f);
        text1.gameObject.SetActive(true);
        yield return new WaitForSeconds(3f);

        for (float i = 1; i > 0; i -= Time.deltaTime / 3)
        {
            hosting.alpha = i;
        }

        text1.gameObject.SetActive(false);
        viewer.RuntimeValue += 3000;
        ObjectManager.Instance.PopObject("AnimatedText", transform.position + Vector3.up).GetComponent<AnimatedText>().SetText($"시청자 +{3000}", Color.white);
        StreamingManager.Instance.donationUI[2].SetActive(false);
        StreamingManager.Instance.donationText[2].text = $"놀란님이 호스팅 하였습니다";
        StreamingManager.Instance.donationImageUI[2].sprite = StreamingManager.Instance.donationImages[3];
        StreamingManager.Instance.donationUI[2].SetActive(true);
        RuntimeManager.PlayOneShot($"event:/SFX/ETC/Donation");
        yield return new WaitForSeconds(.4f);
        viewer.RuntimeValue += 3000;
        ObjectManager.Instance.PopObject("AnimatedText", transform.position + Vector3.up).GetComponent<AnimatedText>().SetText($"시청자 +{3000}", Color.white);
        StreamingManager.Instance.donationUI[3].SetActive(false);
        StreamingManager.Instance.donationText[3].text = $"김반푼이님이 호스팅 하였습니다";
        StreamingManager.Instance.donationImageUI[3].sprite = StreamingManager.Instance.donationImages[3];
        StreamingManager.Instance.donationUI[3].SetActive(true);
        RuntimeManager.PlayOneShot($"event:/SFX/ETC/Donation");
        yield return new WaitForSeconds(.4f);
        viewer.RuntimeValue += 3000;
        ObjectManager.Instance.PopObject("AnimatedText", transform.position + Vector3.up).GetComponent<AnimatedText>().SetText($"시청자 +{3000}", Color.white);
        StreamingManager.Instance.donationUI[4].SetActive(false);
        StreamingManager.Instance.donationText[4].text = $"주르르님이 호스팅 하였습니다";
        StreamingManager.Instance.donationImageUI[4].sprite = StreamingManager.Instance.donationImages[3];
        StreamingManager.Instance.donationUI[4].SetActive(true);
        RuntimeManager.PlayOneShot($"event:/SFX/ETC/Donation");
        yield return new WaitForSeconds(3);
        for (float i = 0; i < 1; i += Time.deltaTime / 3)
        {
            hosting.alpha = i;
        }
        yield return new WaitForSeconds(1f);
        text2.gameObject.SetActive(true);
        yield return new WaitForSeconds(3f);
        for (float i = 1; i > 0; i -= Time.deltaTime / 3)
        {
            hosting.alpha = i;
        }
        text2.gameObject.SetActive(false);
        hosting.gameObject.SetActive(false);
        yield return new WaitForSeconds(1f);
        AudioManager.Instance.PlayRealMusic();
        CinemachineVirtualCamera.m_Lens.OrthographicSize = 카메라사이즈;
        CinemachineTargetGroup.m_Targets[1].target = null;

        Wakgood.Instance.IsSwitching = false;
        Wakgood.Instance.isHealthy = true;
        Wakgood.Instance.IsCollapsed = false;
        Wakgood.Instance.WakgoodMove.MbDashing = false;
        Wakgood.Instance.WakgoodMove.Animator.SetTrigger("WakeUp");
        Wakgood.Instance.WakgoodMove.Animator.SetBool("Move", false);
        Wakgood.Instance.WakgoodMove.PlayerRb.bodyType = RigidbodyType2D.Dynamic;
        Wakgood.Instance.WakgoodMove.enabled = true;
        Wakgood.Instance.wakgoodCollider.enabled = true;
        Wakgood.Instance.ReceiveHeal(100);
        isRealBossFirstDeath = false;

        text3.gameObject.SetActive(true);

        yield return new WaitForSeconds(5f);

        text3.gameObject.SetActive(false);
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