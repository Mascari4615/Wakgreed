using System.Collections;
using Cinemachine;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private GameObject bossSpeedWagon;
    [SerializeField] private TextMeshProUGUI bossSpeedWagonName;
    [SerializeField] private TextMeshProUGUI bossNickNameSpeedWagonName;

    private new CinemachineVirtualCamera camera;
    private CinemachineTargetGroup cinemachineTargetGroup;

    public BossHpBar bossHpBar;

    [SerializeField] private GameObject state;

    public GameObject reloadUI;
    public Image reloadImage;

    [SerializeField] private RectTransform[] weaponUI = new RectTransform[2];
    public Slot[] weaponSprite = new Slot[2];
    public Slot[] weaponSkillQ = new Slot[2];
    public Image[] weaponSkillQCoolTime = new Image[2];
    public Slot[] weaponSkillE = new Slot[2];
    public Image[] weaponSkillECoolTime = new Image[2];

    [SerializeField] private BoolVariable isFocusOnSomething;
    [SerializeField] private TextMeshProUGUI noticeText;

    [SerializeField] private TextMeshProUGUI musicName;
    [SerializeField] private TextMeshProUGUI stageName;
    [SerializeField] private TextMeshProUGUI roomName;
    public GameObject stageSpeedWagon;
    [SerializeField] private TextMeshProUGUI stageNumberText, stageNameCommentText;
    [SerializeField] private GameObject roomClearSpeedWagon;
    [SerializeField] private GameObject rescueSpeedWagon;

    [SerializeField] private BoolVariable isShowingSomething;

    [SerializeField] private IntVariable Goldu;
    [SerializeField] private TextMeshProUGUI resultUptimeText;
    [SerializeField] private TextMeshProUGUI finalStageText;
    [SerializeField] private TextMeshProUGUI totalEquipGoldu;
    [SerializeField] private ItemInventoryUI inventoryUI;

    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private TextMeshProUGUI resultText;

    private void Awake()
    {
        Instance = this;

        camera = GameObject.Find("Cameras").transform.Find("CM Camera").GetComponent<CinemachineVirtualCamera>();
        cinemachineTargetGroup = GameObject.Find("Cameras").transform.Find("CM TargetGroup").GetComponent<CinemachineTargetGroup>();
    }

    public void SetResult(bool Ending = false)
    {
        resultUptimeText.text = StreamingManager.Instance.Uptime;
        finalStageText.text = StageManager.Instance.currentStage.name;
        totalEquipGoldu.text = Goldu.RuntimeValue.ToString();
        inventoryUI.Initialize();

        if (Ending)
        {
            resultText.text = "오뱅알!";
            resultText.color = new Color(0, 200, 120);
        }
    }

    private void Update()
    {
        if (isFocusOnSomething.RuntimeValue)
        {
            if (state.activeSelf) state.SetActive(false);
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.C)) state.SetActive(true);
            else if (Input.GetKeyUp(KeyCode.C)) state.SetActive(false);
        }

        if (Wakgood.Instance.Weapon[Wakgood.Instance.CurWeaponNumber].IsReloading)
        {
            if (!reloadUI.activeSelf)
                reloadUI.SetActive(true);
            reloadImage.fillAmount = Wakgood.Instance.Weapon[Wakgood.Instance.CurWeaponNumber].CurReloadTime / Wakgood.Instance.Weapon[Wakgood.Instance.CurWeaponNumber].reloadTime;
        }
        else if (reloadUI.activeSelf) reloadUI.SetActive(false);

        if (Wakgood.Instance.Weapon[0].skillQ is not null)
            weaponSkillQCoolTime[0].fillAmount =
                Wakgood.Instance.Weapon[0].CurSkillQCoolTime / Wakgood.Instance.Weapon[0].skillQ.coolTime;
        if (Wakgood.Instance.Weapon[0].skillE is not null)
            weaponSkillECoolTime[0].fillAmount =
                Wakgood.Instance.Weapon[0].CurSkillECoolTime / Wakgood.Instance.Weapon[0].skillE.coolTime;
        if (Wakgood.Instance.Weapon[1].skillQ is not null)
            weaponSkillQCoolTime[1].fillAmount =
                Wakgood.Instance.Weapon[1].CurSkillQCoolTime / Wakgood.Instance.Weapon[1].skillQ.coolTime;
        if (Wakgood.Instance.Weapon[1].skillE is not null)
            weaponSkillECoolTime[1].fillAmount =
                Wakgood.Instance.Weapon[1].CurSkillECoolTime / Wakgood.Instance.Weapon[1].skillE.coolTime;
        if (Wakgood.Instance.Weapon[Wakgood.Instance.CurWeaponNumber].attackType.Equals(AttackType.Ranged))
        {
            if (ammoText.gameObject.activeSelf == false)
                ammoText.gameObject.SetActive(true);
            ammoText.text = $"{Wakgood.Instance.Weapon[Wakgood.Instance.CurWeaponNumber].Ammo} / {Wakgood.Instance.Weapon[Wakgood.Instance.CurWeaponNumber].magazine}";
        }
        else
        {
            if (ammoText.gameObject.activeSelf)
                ammoText.gameObject.SetActive(false);
        }
    }

    public IEnumerator SpeedWagon_Stage()
    {
        stageNumberText.text = $"{StageManager.Instance.currentStage.name}";
        stageNameCommentText.text = $"{StageManager.Instance.currentStage.id} 스테이지 - {StageManager.Instance.currentStage.comment}";
        stageSpeedWagon.SetActive(true);
        yield return new WaitForSeconds(3f);
        stageSpeedWagon.SetActive(false);
    }

    public IEnumerator SpeedWagon_BossOn(BossMonster boss)
    {
        isShowingSomething.RuntimeValue = true;
        camera.m_Lens.OrthographicSize = 6;
        cinemachineTargetGroup.m_Targets[0].target = boss.transform;
        Wakgood.Instance.SetRigidBodyType(RigidbodyType2D.Static);
        bossSpeedWagonName.text = boss.mobName;
        bossNickNameSpeedWagonName.text = boss.nickName;
        bossSpeedWagon.SetActive(true);

        yield return new WaitForSeconds(3f);

        isShowingSomething.RuntimeValue = false;

        bossSpeedWagon.SetActive(false);
        Wakgood.Instance.SetRigidBodyType(RigidbodyType2D.Dynamic);
        cinemachineTargetGroup.m_Targets[0].target = Wakgood.Instance.transform;
        cinemachineTargetGroup.m_Targets[1].target = boss.transform;
        camera.m_Lens.OrthographicSize = 12;

        bossHpBar.HpBarOn(boss);
    }

    public IEnumerator SpeedWagon_BossOff(BossMonster boss)
    {
        bossHpBar.HpBarOff();
        camera.m_Lens.OrthographicSize = 6;

        cinemachineTargetGroup.m_Targets[1].target = null;
        cinemachineTargetGroup.m_Targets[0].target = boss.transform;
        Time.timeScale = 0.3f;

        Wakgood.Instance.SetRigidBodyType(RigidbodyType2D.Static);

        yield return new WaitForSecondsRealtime(3f);

        Time.timeScale = 1;
        Wakgood.Instance.SetRigidBodyType(RigidbodyType2D.Dynamic);
        cinemachineTargetGroup.m_Targets[0].target = Wakgood.Instance.transform;
        camera.m_Lens.OrthographicSize = 12;
    }

    public IEnumerator SpeedWagon_RoomClear()
    {
        roomClearSpeedWagon.SetActive(true);
        yield return new WaitForSeconds(2f);
        roomClearSpeedWagon.SetActive(false);
    }

    public IEnumerator SpeedWagon_Rescue()
    {
        rescueSpeedWagon.SetActive(true);
        yield return new WaitForSeconds(2f);
        rescueSpeedWagon.SetActive(false);
    }

    public IEnumerator SwitchWeapon()
    {
        Vector3 weapon1Origin = weaponUI[0].localPosition;
        Vector3 weapon2Origin = weaponUI[1].localPosition;

        for (float i = 0; i <= 1; i += Time.deltaTime * 10)
        {
            weaponUI[0].localPosition = Vector3.Lerp(weapon1Origin, weapon2Origin, i);
            weaponUI[1].localPosition = Vector3.Lerp(weapon2Origin, weapon1Origin, i);
            yield return null;
        }

        weaponUI[0].localPosition = weapon2Origin;
        weaponUI[1].localPosition = weapon1Origin;
    }

    public void SetWeaponUI(int weaponNum, Weapon weapon)
    {
        weaponSprite[weaponNum].SetSlot(weapon);
        weaponSkillQ[weaponNum].gameObject.SetActive(weapon.skillQ);
        if (weapon.skillQ) weaponSkillQ[weaponNum].SetSlot(weapon.skillQ);
        weaponSkillE[weaponNum].gameObject.SetActive(weapon.skillE);
        if (weapon.skillE) weaponSkillE[weaponNum].SetSlot(weapon.skillE);
    }

    [SerializeField] private TextMeshProUGUI viewerUI;

    public void OpenSetting() => SettingManager.Instance.OpenSetting();
    public void SetMusicName(string name) => musicName.text = $"[음악] {name}";
    public void SetStageName(Stage stage) => stageName.text = $"{stage.id} - {stage.name}";
    public void SetStageName(string name) => stageName.text = name;
    public void SetRoomName(Room room) => roomName.text = $"현재 방 이름 : { (room.gameObject.name.Contains("(Clone)") ? room.gameObject.name.Remove(room.gameObject.name.IndexOf("(", StringComparison.Ordinal), 7) : room.gameObject.name)}";
    public void SetRoomName(string name) => roomName.text = name;
    public void SetCurViewerText(string text) => viewerUI.text = text;

    public void StopAllSpeedWagons()
    {
        StopCoroutine(nameof(CantOpenText));
        noticeText.gameObject.SetActive(false);
        StopCoroutine(nameof(SpeedWagon_Stage));
        stageSpeedWagon.SetActive(false);
        StopCoroutine(nameof(SpeedWagon_RoomClear));
        roomClearSpeedWagon.SetActive(false);
        StopCoroutine(nameof(SpeedWagon_Rescue));
        rescueSpeedWagon.SetActive(false);
    }
    private IEnumerator canOpenText;

    public void SpeedWagon_CantOpen()
    {
        if (canOpenText != null) StopCoroutine(canOpenText);
        StartCoroutine(canOpenText = CantOpenText());
    }

    private IEnumerator CantOpenText()
    {
        noticeText.text = "전투 중에는 열 수 없습니다.";
        noticeText.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        noticeText.gameObject.SetActive(false);
    }
}