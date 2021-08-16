using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public enum Area
{
    Normal,
    Test
}

public class AreaTweener : MonoBehaviour
{
    private static AreaTweener instance = null;
    public static AreaTweener Instance { get { return instance; } }
    private void Awake() { instance = this; }

    public Area curArea = Area.Normal;
    public GameObject curAreaGO;
    [SerializeField] private CinemachineVirtualCamera cuRvirtualCamera;
    public GameObject nornalArea;
    public GameObject testArea;
    [SerializeField] private CinemachineVirtualCamera textAreacinemachineVirtualCamera;
    public Text speedwagon;
    private Coroutine speedwagon_co;

    public void TweenArea(Area targetArea)
    {
        //if (curArea != Area.Normal) curAreaGO.SetActive(false);

        //if (targetArea != Area.Normal)
        //{
        //    if (targetArea == Area.Test) { curAreaGO = testArea; }
        //    curAreaGO.SetActive(true);
        //}

        if (targetArea == curArea) return;

        curAreaGO.SetActive(false);
        cuRvirtualCamera.Priority = -100;

        if (targetArea == Area.Test) { curAreaGO = testArea; cuRvirtualCamera = textAreacinemachineVirtualCamera; }
        else if (targetArea == Area.Normal) { curAreaGO = nornalArea; }

        curAreaGO.SetActive(true);
        if(targetArea != Area.Normal) cuRvirtualCamera.Priority = 400;

        curArea = targetArea;

        if (curArea != Area.Normal)
        {
            if (speedwagon_co != null) StopCoroutine(speedwagon_co);
            speedwagon_co = StartCoroutine(AreaSpeedWagon());
        }
    }

    public IEnumerator AreaSpeedWagon()
    {
        speedwagon.text = curArea.ToString();
        speedwagon.gameObject.SetActive(true);
        yield return new WaitForSeconds(2f);
        speedwagon.gameObject.SetActive(false);
    }
}
