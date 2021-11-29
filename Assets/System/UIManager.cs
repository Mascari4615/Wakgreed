using System.Collections;
using Cinemachine;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private GameObject bossSpeedWagon;

    private CinemachineTargetGroup cinemachineTargetGroup;

    public BossHpBar bossHpBar;

    [SerializeField] private GameObject state;

    public GameObject reloadUI;
    public Image reloadImage;

    [SerializeField] private RectTransform[] weaponUI = new RectTransform[2];
    public Slot[] weaponSprite = new Slot[2];
    public Slot[] weaponSkillQ= new Slot[2];
    public Image[] weaponSkillQCoolTime= new Image[2];
    public Slot[] weaponSkillE= new Slot[2];
    public Image[] weaponSkillECoolTime = new Image[2];

    [SerializeField] private BoolVariable isFocusOnSomething;

    private void Awake()
    {
        Instance = this;

        cinemachineTargetGroup =
            GameObject.Find("Cameras").transform.GetChild(3).GetComponent<CinemachineTargetGroup>();
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

        if (Wakgood.Instance.CurWeapon.IsReloading)
        {
            if (!reloadUI.activeSelf)
                reloadUI.SetActive(true);
            reloadImage.fillAmount = Wakgood.Instance.CurWeapon.CurReloadTime / Wakgood.Instance.CurWeapon.reloadTime;
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
    }

    public IEnumerator SpeedWagon_Boss(GameObject boss)
    {
        cinemachineTargetGroup.m_Targets[0].target = boss.transform;
        Wakgood.Instance.SetRigidBodyType(RigidbodyType2D.Static);
        bossSpeedWagon.gameObject.SetActive(true);

        yield return new WaitForSeconds(3f);

        bossSpeedWagon.gameObject.SetActive(false);
        Wakgood.Instance.SetRigidBodyType(RigidbodyType2D.Dynamic);
        cinemachineTargetGroup.m_Targets[0].target = Wakgood.Instance.transform;
    }

    public IEnumerator SwitchWeapon()
    {
        Vector3 weapon1Origin = weaponUI[0].localPosition;
        Vector3 weapon2Origin = weaponUI[1].localPosition;
        
        for (float i = 0; i < .20f; i += Time.deltaTime)
        {
            weaponUI[0].localPosition = Vector3.Lerp(weapon1Origin, weapon2Origin, i * 6);
            weaponUI[1].localPosition = Vector3.Lerp(weapon2Origin, weapon1Origin, i * 6);
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

    public void OpenSetting()
    {
        SettingManager.Instance.SettingPanel.SetActive(true);
    }
}