using System.Collections;
using Cinemachine;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [FormerlySerializedAs("SpeedWagons")] [SerializeField] private GameObject[] speedWagons;
    [SerializeField] private GameObject bossSpeedWagon;

    private CinemachineTargetGroup cinemachineTargetGroup;

    public GameObject bossHpBar;
    public GameObject redParent;

    [SerializeField] private GameObject state;

    public TextMeshProUGUI aaa;

    public GameObject reloadUI;
    public Image reloadImage;

    [SerializeField] private RectTransform weapon1;
    [SerializeField] private RectTransform weapon2;
    public Slot weapon1Sprite;
    public Slot weapon2Sprite;
    public Slot weapon1SkillQ;
    public Image weapon1SkillQCoolTime;
    public Slot weapon1SkillE;
    public Image weapon1SkillECoolTime;
    public Slot weapon2SkillQ;
    public Image weapon2SkillQCoolTime;
    public Slot weapon2SkillE;
    public Image weapon2SkillECoolTime;

    private void Awake()
    {
        Instance = this;

        cinemachineTargetGroup = GameObject.Find("Cameras").transform.GetChild(3).GetComponent<CinemachineTargetGroup>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C)) state.SetActive(true);
        else if (Input.GetKeyUp(KeyCode.C)) state.SetActive(false);

        if (Wakgood.Instance.CurWeapon.IsReloading) 
        {if (!reloadUI.activeSelf) reloadUI.SetActive(true); reloadImage.fillAmount = Wakgood.Instance.CurWeapon.CurReloadTime / Wakgood.Instance.CurWeapon.reloadTime;}
        else if (reloadUI.activeSelf) reloadUI.SetActive(false);
        if (Wakgood.Instance.Weapon1.skillQ is not null) weapon1SkillQCoolTime.fillAmount = Wakgood.Instance.Weapon1.CurSkillQCoolTime / Wakgood.Instance.Weapon1.skillQ.coolTime;
        if (Wakgood.Instance.Weapon1.skillE is not null) weapon1SkillECoolTime.fillAmount = Wakgood.Instance.Weapon1.CurSkillECoolTime/ Wakgood.Instance.Weapon1.skillE.coolTime;
        if (Wakgood.Instance.Weapon2.skillQ is not null) weapon2SkillQCoolTime.fillAmount = Wakgood.Instance.Weapon2.CurSkillQCoolTime / Wakgood.Instance.Weapon2.skillQ.coolTime;
        if (Wakgood.Instance.Weapon2.skillE is not null) weapon2SkillECoolTime.fillAmount = Wakgood.Instance.Weapon2.CurSkillECoolTime / Wakgood.Instance.Weapon2.skillE.coolTime;
    }

    public IEnumerator SpeedWagon_Boss(GameObject boss)
    {
        cinemachineTargetGroup.m_Targets[0].target = boss.transform;
        Wakgood.Instance.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        bossSpeedWagon.gameObject.SetActive(true);

        yield return new WaitForSeconds(3f);

        bossSpeedWagon.gameObject.SetActive(false);
        Wakgood.Instance.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        cinemachineTargetGroup.m_Targets[0].target = Wakgood.Instance.transform;
    }

    public IEnumerator SwitchWeapon()
    {
        Vector3 weapon1Origin = weapon1.localPosition;
        Vector3 weapon2Origin = weapon2.localPosition;
        for (float i = 0; i < .20f; i += Time.deltaTime)
        {
            weapon1.localPosition = Vector3.Lerp(weapon1Origin, weapon2Origin, i * 6);
            weapon2.localPosition = Vector3.Lerp(weapon2Origin, weapon1Origin, i * 6);
            yield return null;
        }
    }

    public void OpenSetting()
    {
        SettingManager.Instance.SettingPanel.SetActive(true);
    }
}
