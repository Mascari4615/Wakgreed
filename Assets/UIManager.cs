using System.Collections;
using Cinemachine;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get { return instance; } }
    private static UIManager instance;

    [SerializeField] private GameObject[] SpeedWagons;
    [SerializeField] private GameObject bossSpeedWagon;

    private CinemachineTargetGroup cinemachineTargetGroup;

    public GameObject bossHpBar;
    public GameObject redParent;

    [SerializeField] private GameObject needMoreNyang;

    private void Awake()
    {
        instance = this;

        cinemachineTargetGroup = GameObject.Find("Cameras").transform.GetChild(3).GetComponent<CinemachineTargetGroup>();
    }

    public IEnumerator SpeedWagon_Boss(GameObject boss)
    {
        boss.GetComponent<Monster>().enabled = false;
        cinemachineTargetGroup.m_Targets[0].target = boss.transform;
        TravellerController.Instance.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Static;
        bossSpeedWagon.gameObject.SetActive(true);

        yield return new WaitForSeconds(3f);

        bossSpeedWagon.gameObject.SetActive(false);
        TravellerController.Instance.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        cinemachineTargetGroup.m_Targets[0].target = TravellerController.Instance.transform;
        boss.GetComponent<Monster>().enabled = true;
    }

    public IEnumerator NeedMoreNyang()
    {
        needMoreNyang.SetActive(true);
        yield return new WaitForSeconds(1);
        needMoreNyang.SetActive(false);
    }
}
