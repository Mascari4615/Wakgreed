using Cinemachine;
using System.Collections;
using UnityEngine;
using TMPro;

public enum AreaType
{
    Normal,
    Restaurant,
    Shop
}

public class AreaTweener : MonoBehaviour
{
    public static AreaTweener Instance { get; private set; } = null;

    private CinemachineConfiner2D cinemachineConfiner2D;
    [SerializeField] private TextMeshProUGUI speedwagon;
    private IEnumerator speedwagon_co;

    private void Awake()
    {
        Instance = this;
        cinemachineConfiner2D = GameObject.Find("CM Camera").GetComponent<CinemachineConfiner2D>();
        speedwagon_co = AreaSpeedWagon("Temp");
    }

    public void ChangeArea(Transform areaDoor)
    {
        AreaDoor targetArea = areaDoor.GetComponent<AreaDoor>();
        targetArea.originalAreaObject.SetActive(false);
        targetArea.targetAreaObject.SetActive(true);
        cinemachineConfiner2D.m_BoundingShape2D = targetArea.Equals(AreaType.Normal) ? null : targetArea.compositeCollider2d;
        StopCoroutine(speedwagon_co);
        StartCoroutine(speedwagon_co = AreaSpeedWagon(targetArea.targetAreaType.ToString()));
    }

    private IEnumerator AreaSpeedWagon(string text)
    {
        speedwagon.text = text;
        speedwagon.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        speedwagon.gameObject.SetActive(false);
    }
}
