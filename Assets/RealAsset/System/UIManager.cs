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
}
