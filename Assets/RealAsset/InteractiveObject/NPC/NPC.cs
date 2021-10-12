using Cinemachine;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class NPC : InteractiveObject
{
    [SerializeField] protected CinemachineVirtualCamera cinemachineVirtualCamera;

    [SerializeField] protected GameObject ui;

    [SerializeField] protected GameObject chat;
    [SerializeField] private TextMeshProUGUI chatText;
    [SerializeField] private Transform temp;

    [TextArea] [SerializeField] private List<string> comment;

    private bool isTalking = false, inputSkip = false;

    private void Awake()
    {
        cinemachineVirtualCamera.Follow = GameObject.Find("CM TargetGroup").transform;
        chatText.text = "";
    }

    public override void Interaction()
    {
        if (!isTalking)
        {
            isTalking = true;
            inputSkip = false;

            StopAllCoroutines();
            StartCoroutine(OnType());
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameObject.Find("CM TargetGroup").GetComponent<CinemachineTargetGroup>().m_Targets[1].target = transform;
            GameObject.Find("CM TargetGroup").GetComponent<CinemachineTargetGroup>().m_Targets[1].weight = 5;
            //GameObject.Find("CM Camera").GetComponent<CinemachineVirtualCamera>().m_Lens.OrthographicSize = 9;
            cinemachineVirtualCamera.Priority = 200;
        }
    }

    protected virtual void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameObject.Find("CM TargetGroup").GetComponent<CinemachineTargetGroup>().m_Targets[1].target = null;
            GameObject.Find("CM TargetGroup").GetComponent<CinemachineTargetGroup>().m_Targets[1].weight = 1;
            cinemachineVirtualCamera.Priority = -100;
            ui.SetActive(false);
            chat.SetActive(false);

            StopAllCoroutines();
            isTalking = false;
            inputSkip = false;
        }
    }

    private IEnumerator OnType()
    {
        WaitForSeconds ws005 = new(0.05f);
        WaitForSeconds ws02 = new(0.2f);

        if (comment.Count > 0)
        {
            IEnumerator CheckSkip;
            chat.SetActive(true);
            for (int i = 0; i < comment.Count; i++)
            {
                chatText.text = "";

                StartCoroutine(CheckSkip = CheckSkipInput());
                foreach (char item in comment[i])
                {
                    if (!inputSkip)
                    {
                        chatText.text += item;
                        RuntimeManager.PlayOneShot("event:/SFX/ETC/NPC_Temp", transform.position);
                        yield return ws005;
                    }
                    else
                    {
                        chatText.text = comment[i];
                        break;
                    }
                }
                StopCoroutine(CheckSkip);
                inputSkip = false;

                yield return ws02;
                yield return StartCoroutine(WaitForTheInput());
            }
            chat.SetActive(false);
        }

        if (ui is not null && ui.activeSelf is false) ui.SetActive(true);
    }

    private IEnumerator WaitForTheInput()
    {
        do yield return null;
        while (!Input.GetKeyDown(KeyCode.F));
    }

    private IEnumerator CheckSkipInput()
    {
        do yield return null;
        while (!Input.GetKeyDown(KeyCode.F));
        inputSkip = true;
    }
}