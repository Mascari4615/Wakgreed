using System.Collections;
using Cinemachine;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get { return instance; } }
    private static UIManager instance;

    [SerializeField] private GameObject[] SpeedWagons;
    [SerializeField] private GameObject bossSpeedWagon;

    private CinemachineTargetGroup cinemachineTargetGroup;

    public GameObject bossHpBar;
    public GameObject redParent;

    [SerializeField] private GameObject state;

    public Restaurant restaurant;

    public TextMeshProUGUI aaa;

    public GameObject reloadUI;
    public Image reloadImage;

    [SerializeField] private RectTransform weapon1;
    [SerializeField] private RectTransform weapon2;
    public Image weapon1Sprite;
    public Image weapon2Sprite;
    public GameObject weapon1SkillQ;
    public Image weapon1SkillQSprite;
    public Image weapon1SkillQCoolTime;
    public GameObject weapon1SkillE;
    public Image weapon1SkillESprite;
    public Image weapon1SkillECoolTime;
    public GameObject weapon2SkillQ;
    public Image weapon2SkillQSprite;
    public Image weapon2SkillQCoolTime;
    public GameObject weapon2SkillE;
    public Image weapon2SkillESprite;
    public Image weapon2SkillECoolTime;

    private void Awake()
    {
        instance = this;

        cinemachineTargetGroup = GameObject.Find("Cameras").transform.GetChild(3).GetComponent<CinemachineTargetGroup>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C)) state.SetActive(true);
        else if (Input.GetKeyUp(KeyCode.C)) state.SetActive(false);
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
        Debug.Log("Start");
        Vector3 weapon1Origin = weapon1.localPosition;
        Vector3 weapon2Origin = weapon2.localPosition;
        for (float i = 0; i < .20f; i += Time.deltaTime)
        {
            weapon1.localPosition = Vector3.Lerp(weapon1Origin, weapon2Origin, i * 6);
            weapon2.localPosition = Vector3.Lerp(weapon2Origin, weapon1Origin, i * 6);
            yield return null;
        }
        Debug.Log("End");
    }
}
