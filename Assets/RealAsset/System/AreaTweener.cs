using Cinemachine;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public enum AreaType
{
    Normal,
    Test
}

public class AreaTweener : MonoBehaviour
{
    private static AreaTweener instance = null;
    public static AreaTweener Instance { get { return instance; } }

    private AreaType curAreaType = AreaType.Normal;
    private Area curArea;
    [SerializeField] private Text speedwagon;
    private Coroutine speedwagon_co;

    private void Awake()
    {
        instance = this;
    }

    public void NormalToArea(Transform target = null)
    {
        if (curAreaType == AreaType.Test)
        {
            return;
        }
        curAreaType = AreaType.Test;

        curArea = target.parent.GetComponent<Area>();
        curArea.A.SetActive(false);
        curArea.B.SetActive(true);
        GameObject.Find("CM Camera").GetComponent<CinemachineConfiner2D>().m_BoundingShape2D = curArea.compositeCollider2d;

        if (speedwagon_co != null)
        {
            StopCoroutine(speedwagon_co);
        }
        speedwagon_co = StartCoroutine(AreaSpeedWagon());
    }

    public void AreaToNormal()
    {
        if (curAreaType == AreaType.Normal)
        {
            return;
        }
        curAreaType = AreaType.Normal;

        GameObject.Find("CM Camera").GetComponent<CinemachineConfiner2D>().m_BoundingShape2D = null;
        curArea.A.SetActive(true);
        curArea.B.SetActive(false);
    }

    public IEnumerator AreaSpeedWagon()
    {
        speedwagon.text = curAreaType.ToString();
        speedwagon.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        speedwagon.gameObject.SetActive(false);
    }
}
