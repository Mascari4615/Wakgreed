using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject bossSpeedWagon;
    [SerializeField] private TextMeshProUGUI bossSpeedWagonName;
    [SerializeField] private TextMeshProUGUI bossNickNameSpeedWagonName;
    [SerializeField] private BossHpBar bossHpBar;
    [SerializeField] private GameObject state;
    [SerializeField] private GameObject reloadUI;
    [SerializeField] private Image reloadImage;
    [SerializeField] private Image wakduImage;
    [SerializeField] private RectTransform[] weaponUI = new RectTransform[2];
    [SerializeField] private Slot[] weaponSprite = new Slot[2];
    [SerializeField] private Slot[] weaponSpecialAttack = new Slot[2];
    [SerializeField] private Slot[] weaponSkillQ = new Slot[2];
    [SerializeField] private Slot[] weaponSkillE = new Slot[2];
    [SerializeField] private Image[] weaponSpecialAttackCoolTime = new Image[2];
    [SerializeField] private Image[] weaponSkillQCoolTime = new Image[2];
    [SerializeField] private Image[] weaponSkillECoolTime = new Image[2];
    [SerializeField] private BoolVariable isFocusOnSomething;
    [SerializeField] private TextMeshProUGUI noticeText;
    [SerializeField] private TextMeshProUGUI musicName;
    [SerializeField] private TextMeshProUGUI stageName;
    [SerializeField] private TextMeshProUGUI roomName;
    [SerializeField] private GameObject stageSpeedWagon;
    [SerializeField] private TextMeshProUGUI stageNumberText, stageNameCommentText;
    [SerializeField] private GameObject roomClearSpeedWagon;
    [SerializeField] private GameObject rescueSpeedWagon;
    [SerializeField] private BoolVariable isShowingSomething;
    [SerializeField] private TextMeshProUGUI resultUptimeText;
    [SerializeField] private TextMeshProUGUI finalStageText;
    [SerializeField] private TextMeshProUGUI totalEquipGoldu;
    [SerializeField] private ItemInventoryUI inventoryUI;
    [SerializeField] private TextMeshProUGUI viewerUI;
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private TextMeshProUGUI resultText;
    private IEnumerator canOpenText;
    public static UIManager Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        SetStageName("마을");
        SetRoomName("마을");
        SetCurViewerText("뱅온 전!");
    }

    private void Update()
    {
        if (isFocusOnSomething.RuntimeValue)
        {
            if (state.activeSelf)
            {
                state.SetActive(false);
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                state.SetActive(true);
            }
            else if (Input.GetKeyUp(KeyCode.C))
            {
                state.SetActive(false);
            }
        }

        if (Wakgood.Instance.Weapon[Wakgood.Instance.CurWeaponNumber].IsReloading)
        {
            if (!reloadUI.activeSelf)
            {
                reloadUI.SetActive(true);
            }

            reloadImage.fillAmount = Wakgood.Instance.Weapon[Wakgood.Instance.CurWeaponNumber].CurReloadTime /
                                     Wakgood.Instance.Weapon[Wakgood.Instance.CurWeaponNumber].reloadTime;
        }
        else if (reloadUI.activeSelf)
        {
            reloadUI.SetActive(false);
        }

        UpdateWeaponSkillCoolUI(0);
        UpdateWeaponSkillCoolUI(1);

        if (Wakgood.Instance.Weapon[Wakgood.Instance.CurWeaponNumber].attackType.Equals(AttackType.Ranged))
        {
            if (ammoText.gameObject.activeSelf == false)
            {
                ammoText.gameObject.SetActive(true);
            }

            ammoText.text =
                $"{Wakgood.Instance.Weapon[Wakgood.Instance.CurWeaponNumber].Ammo} / {Wakgood.Instance.Weapon[Wakgood.Instance.CurWeaponNumber].magazine + Wakgood.Instance.bonusAmmo.RuntimeValue}";
        }
        else
        {
            if (ammoText.gameObject.activeSelf)
            {
                ammoText.gameObject.SetActive(false);
            }
        }
    }

    public void SetResult(int goldu, bool Ending = false)
    {
        resultUptimeText.text = StreamingManager.Instance.Uptime;
        finalStageText.text = StageManager.Instance.currentStage.name;
        totalEquipGoldu.text = goldu.ToString();
        inventoryUI.Initialize();

        if (Ending)
        {
            resultText.text = "오뱅알!";
            resultText.color = new Color(200 / 255f, 255 / 255f, 130 / 255f);
        }
        else
        {
            resultText.text = "오뱅창!";
            resultText.color = new Color(255 / 255f, 90 / 255f, 90 / 255f);
        }
    }

    private void UpdateWeaponSkillCoolUI(int weaponNum)
    {
        Weapon weapon = Wakgood.Instance.Weapon[weaponNum];

        if (weapon.specialAttack is not null)
        {
            weaponSpecialAttackCoolTime[weaponNum].fillAmount =
                weapon.CurSpecialAttackCoolTime / (weapon.specialAttack.coolTime *
                                                   (1 - (Wakgood.Instance.skillCollBonus.RuntimeValue / 100)));
        }

        if (weapon.skillQ is not null)
        {
            weaponSkillQCoolTime[weaponNum].fillAmount =
                weapon.CurSkillQCoolTime /
                (weapon.skillQ.coolTime * (1 - (Wakgood.Instance.skillCollBonus.RuntimeValue / 100)));
        }

        if (weapon.skillE is not null)
        {
            weaponSkillECoolTime[weaponNum].fillAmount =
                weapon.CurSkillECoolTime /
                (weapon.skillE.coolTime * (1 - (Wakgood.Instance.skillCollBonus.RuntimeValue / 100)));
        }
    }

    public IEnumerator SpeedWagon_Stage()
    {
        Stage stage = StageManager.Instance.currentStage;

        stageNumberText.text = $"{stage.name}";
        stageNameCommentText.text = $"{stage.id} 스테이지 - {stage.comment}";
        stageSpeedWagon.SetActive(true);
        yield return new WaitForSeconds(3f);
        stageSpeedWagon.SetActive(false);
    }

    public IEnumerator SpeedWagon_BossOn(BossMonster boss)
    {
        isShowingSomething.RuntimeValue = true;
        GameManager.Instance.CinemachineVirtualCamera.m_Lens.OrthographicSize = 6;
        GameManager.Instance.CinemachineTargetGroup.m_Targets[0].target = boss.transform;
        Wakgood.Instance.SetRigidBodyType(RigidbodyType2D.Static);
        bossSpeedWagonName.text = boss.mobName;
        bossNickNameSpeedWagonName.text = boss.nickName;
        bossSpeedWagon.SetActive(true);

        yield return new WaitForSeconds(3f);

        isShowingSomething.RuntimeValue = false;

        bossSpeedWagon.SetActive(false);
        Wakgood.Instance.SetRigidBodyType(RigidbodyType2D.Dynamic);
        GameManager.Instance.CinemachineTargetGroup.m_Targets[0].target = Wakgood.Instance.transform;
        GameManager.Instance.CinemachineTargetGroup.m_Targets[1].target = boss.transform;
        GameManager.Instance.CinemachineVirtualCamera.m_Lens.OrthographicSize = 12;

        bossHpBar.HpBarOn(boss);
    }

    public void BossHpBarOff()
    {
        bossHpBar.HpBarOff();
    }

    public IEnumerator SpeedWagon_BossOff(BossMonster boss)
    {
        bossHpBar.HpBarOff();
        GameManager.Instance.CinemachineVirtualCamera.m_Lens.OrthographicSize = 6;

        GameManager.Instance.CinemachineTargetGroup.m_Targets[1].target = null;
        GameManager.Instance.CinemachineTargetGroup.m_Targets[0].target = boss.transform;
        Time.timeScale = 0.3f;

        Wakgood.Instance.SetRigidBodyType(RigidbodyType2D.Static);

        yield return new WaitForSecondsRealtime(3f);

        Time.timeScale = 1;
        Wakgood.Instance.SetRigidBodyType(RigidbodyType2D.Dynamic);
        GameManager.Instance.CinemachineTargetGroup.m_Targets[0].target = Wakgood.Instance.transform;
        GameManager.Instance.CinemachineVirtualCamera.m_Lens.OrthographicSize = 12;
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
        weaponSpecialAttack[weaponNum].gameObject.SetActive(weapon.specialAttack);
        if (weapon.specialAttack)
        {
            weaponSpecialAttack[weaponNum].SetSlot(weapon.specialAttack);
        }

        weaponSkillQ[weaponNum].gameObject.SetActive(weapon.skillQ);
        if (weapon.skillQ)
        {
            weaponSkillQ[weaponNum].SetSlot(weapon.skillQ);
        }

        weaponSkillE[weaponNum].gameObject.SetActive(weapon.skillE);
        if (weapon.skillE)
        {
            weaponSkillE[weaponNum].SetSlot(weapon.skillE);
        }
    }

    public void OpenSetting()
    {
        SettingManager.Instance.OpenSetting();
    }

    public void SetMusicName(string name)
    {
        musicName.text = $"[음악] {name}";
    }

    public void SetStageName(Stage stage)
    {
        stageName.text = $"{stage.id} - {stage.name}";
    }

    public void SetStageName(string name)
    {
        stageName.text = name;
    }

    public void SetRoomName(Room room)
    {
        roomName.text =
            $"현재 방 이름 : {(room.gameObject.name.Contains("(Clone)") ? room.gameObject.name.Remove(room.gameObject.name.IndexOf("(", StringComparison.Ordinal), 7) : room.gameObject.name)}";
    }

    public void SetRoomName(string name)
    {
        roomName.text = name;
    }

    public void SetCurViewerText(string text)
    {
        viewerUI.text = text;
    }

    public void SetWakduSprite(Sprite sprite)
    {
        wakduImage.sprite = sprite;
    }

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

    public void SpeedWagon_CantOpen()
    {
        if (canOpenText != null)
        {
            StopCoroutine(canOpenText);
        }

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