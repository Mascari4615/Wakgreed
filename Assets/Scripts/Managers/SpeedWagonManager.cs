using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class SpeedWagonManager : MonoBehaviour
{
    private static SpeedWagonManager instance;
    [HideInInspector] public static SpeedWagonManager Instance { get { return instance; } }

    [SerializeField] private GameObject stageSpeedWagon = null;
    [SerializeField] private Text stageNumberText, stageNameText = null;
    [SerializeField] private GameObject bossSpeedWagon = null;
    [SerializeField] private GameObject roomClearSpeedWagon = null;
    
    private void Awake()
    {
        instance = this;
    }

    public IEnumerator StageSpeedWagon(int currentStageNumber)
    {
        stageSpeedWagon.SetActive(true);
        stageNumberText.text = currentStageNumber.ToString();
        stageNameText.text = StageManager.Instace.stageData.stageName;
        yield return new WaitForSeconds(2f);
        stageSpeedWagon.SetActive(false);
    }

    public IEnumerator BossSpeedWagon()
    {
        GameObject.Find("SlimeKing(Clone)").GetComponent<SlimeKing>().enabled = false;
        GameObject.Find("SlimeKing(Clone)").transform.Find("CM Camera1").GetComponent<CinemachineVirtualCamera>().Priority = 100;
        Traveller.Instance.playerRB.bodyType = RigidbodyType2D.Static;
        Traveller.Instance.enabled = false;
        bossSpeedWagon.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(3f);
        bossSpeedWagon.gameObject.SetActive(false);
        Traveller.Instance.enabled = true;
        Traveller.Instance.playerRB.bodyType = RigidbodyType2D.Dynamic;
        GameObject.Find("SlimeKing(Clone)").transform.Find("CM Camera1").GetComponent<CinemachineVirtualCamera>().Priority = 0;
        GameObject.Find("SlimeKing(Clone)").GetComponent<SlimeKing>().enabled = true;
    }

    public IEnumerator RoomClearSpeedWagon()
    {
        roomClearSpeedWagon.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(2f);
        roomClearSpeedWagon.gameObject.SetActive(false);
    }
}
