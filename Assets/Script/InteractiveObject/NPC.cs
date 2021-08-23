using UnityEngine;
using Cinemachine;

public abstract class NPC : InteractiveObject
{
    [SerializeField] protected GameObject canvas;
    [SerializeField] private GameObject ui;
    [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;

    private void Awake()
    {
        cinemachineVirtualCamera.Follow = GameObject.Find("CM TargetGroup").transform;
        canvas.GetComponent<Canvas>().worldCamera = Camera.main;
    }

    public override void Interaction()
    {
        //canvas.SetActive(true);
        ui.SetActive(true);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameObject.Find("CM TargetGroup").GetComponent<CinemachineTargetGroup>().m_Targets[1].target = transform;
            GameObject.Find("CM TargetGroup").GetComponent<CinemachineTargetGroup>().m_Targets[1].weight = 5;
            //GameObject.Find("CM Camera").GetComponent<CinemachineVirtualCamera>().m_Lens.OrthographicSize = 9;
            cinemachineVirtualCamera.Priority = 200;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameObject.Find("CM TargetGroup").GetComponent<CinemachineTargetGroup>().m_Targets[1].target = null;
            GameObject.Find("CM TargetGroup").GetComponent<CinemachineTargetGroup>().m_Targets[1].weight = 1;
            cinemachineVirtualCamera.Priority = -100;
            //canvas.SetActive(false);
            ui.SetActive(false);
        }
    }
}
