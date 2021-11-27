using Cinemachine;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class NPC : InteractiveObject
{
    [TextArea] [SerializeField] private List<string> comment;
    protected CinemachineVirtualCamera cinemachineVirtualCamera;
    protected GameObject ui;
    protected GameObject chat;
    private TextMeshProUGUI chatText;
    private CinemachineTargetGroup cinemachineTargetGroup;
    private bool isTalking, inputSkip;
    private WaitForSeconds ws005 = new(0.05f);
    private WaitForSeconds ws02 = new(0.2f);

    private void Awake()
    {
        cinemachineVirtualCamera = transform.Find("CM").GetComponent<CinemachineVirtualCamera>();
        ui = transform.Find("CustomUI").gameObject;
        chat = transform.Find("DefaultUI").Find("Chat").gameObject;
        chatText = chat.transform.GetChild(1).GetChild(2).GetComponent<TextMeshProUGUI>();
        cinemachineVirtualCamera.Follow = GameObject.Find("CM TargetGroup").transform;
        cinemachineTargetGroup = GameObject.Find("CM TargetGroup").GetComponent<CinemachineTargetGroup>();
    }

    public override void Interaction()
    {
        if (isTalking)
        {
            return;
        }

        isTalking = true;
        inputSkip = false;

        StopAllCoroutines();
        StartCoroutine(OnType());
    }
    
    protected virtual void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }
        
        cinemachineTargetGroup.m_Targets[1].target = null;
        cinemachineTargetGroup.m_Targets[1].weight = 1;
        cinemachineVirtualCamera.Priority = -100;
        ui.SetActive(false);
        chat.SetActive(false);

        StopAllCoroutines();
        isTalking = false;
        inputSkip = false;
    }

    private IEnumerator OnType()
    {
        cinemachineTargetGroup.m_Targets[1].target = transform;
        cinemachineTargetGroup.m_Targets[1].weight = 5;
        //GameObject.Find("CM Camera").GetComponent<CinemachineVirtualCamera>().m_Lens.OrthographicSize = 9;
        cinemachineVirtualCamera.Priority = 200;
        
        if (comment.Count > 0)
        {
            chat.SetActive(true);
            foreach (string t in comment)
            {
                chatText.text = "";

                IEnumerator checkSkip;
                StartCoroutine(checkSkip = CheckSkipInput());
                foreach (char item in t)
                {
                    if (!inputSkip)
                    {
                        chatText.text += item;
                        RuntimeManager.PlayOneShot("event:/SFX/ETC/NPC_Temp", transform.position);
                        yield return ws005;
                    }
                    else
                    {
                        chatText.text = t;
                        break;
                    }
                }
                StopCoroutine(checkSkip);
                inputSkip = false;

                yield return ws02;
                
                do yield return null;
                while (!Input.GetKeyDown(KeyCode.F));
            }
            chat.SetActive(false);
        }
        
        cinemachineTargetGroup.m_Targets[1].target = null;
        cinemachineTargetGroup.m_Targets[1].weight = 1;
        cinemachineVirtualCamera.Priority = -100;
        isTalking = false;
        
        ui.SetActive(ui.activeSelf == false);
    }

    private IEnumerator CheckSkipInput()
    {
        do yield return null;
        while (!Input.GetKeyDown(KeyCode.F));
        inputSkip = true;
    }
}