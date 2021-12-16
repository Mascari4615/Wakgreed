using Cinemachine;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class NPC : InteractiveObject
{
    [TextArea] [SerializeField] private List<string> firstComment;
    protected CinemachineVirtualCamera cvm1;
    [TextArea] [SerializeField] private List<string> randomComment;
    protected CinemachineVirtualCamera cvm2;
    protected GameObject ui;
    protected GameObject chat;
    protected bool canOpenUI = true;
    private TextMeshProUGUI chatText;
    protected CinemachineTargetGroup cinemachineTargetGroup;
    private bool isFirstTalking = true, isTalking, inputSkip;
    private readonly WaitForSeconds ws005 = new(0.05f);
    private readonly WaitForSeconds ws02 = new(0.2f);
    [SerializeField] private BoolVariable isShowingSomething;
    protected Animator animator;

    protected virtual void Awake()
    {
        cvm1 = transform.Find("CM").GetComponent<CinemachineVirtualCamera>();
        cvm2 = transform.Find("CM2").GetComponent<CinemachineVirtualCamera>();
        ui = transform.Find("CustomUI").gameObject;
        chat = transform.Find("DefaultUI").Find("Chat").gameObject;
        chatText = chat.transform.GetChild(1).GetChild(2).GetComponent<TextMeshProUGUI>();
        cvm1.Follow = GameObject.Find("Cameras").transform.GetChild(3);
        cinemachineTargetGroup = GameObject.Find("Cameras").transform.GetChild(3).GetComponent<CinemachineTargetGroup>();
        animator = GetComponent<Animator>();
    }

    public override void Interaction()
    {
        if (isTalking)
        {
            return;
        }

        isShowingSomething.RuntimeValue = true;
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
        cvm1.Priority = -100;
        cvm2.Priority = -100;
        ui.SetActive(false);
        chat.SetActive(false);

        StopAllCoroutines();
        isTalking = false;
        inputSkip = false;
        isShowingSomething.RuntimeValue = false;
    }

    public void FocusOff()
    {
        cinemachineTargetGroup.m_Targets[1].target = null;
        cinemachineTargetGroup.m_Targets[1].weight = 1;
        cvm1.Priority = -100;
        cvm2.Priority = -100;
        ui.SetActive(false);
        chat.SetActive(false);

        StopAllCoroutines();
        isTalking = false;
        inputSkip = false;
        isShowingSomething.RuntimeValue = false;
    }

    private IEnumerator OnType()
    {
        cinemachineTargetGroup.m_Targets[1].target = transform;
        cinemachineTargetGroup.m_Targets[1].weight = 5;
        //GameObject.Find("CM Camera").GetComponent<CinemachineVirtualCamera>().m_Lens.OrthographicSize = 9;
        cvm1.Priority = 200;

        List<string> tempComment;

        if (isFirstTalking)
        {
            isFirstTalking = false;
            tempComment = firstComment;
        }
        else
        {
            tempComment = new List<string>(1);
            tempComment.Add(randomComment[Random.Range(0, randomComment.Count)]);
        }

        if (tempComment.Count > 0)
        {
            chat.SetActive(true);
            foreach (string t in tempComment)
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
        cvm1.Priority = -100;
        isTalking = false;
        
        if (canOpenUI)
        {
            ui.SetActive(true);
            cvm2.Priority = 300;
        }
        else
        {
            FocusOff();
        }
    }

    private IEnumerator CheckSkipInput()
    {
        do yield return null;
        while (!Input.GetKeyDown(KeyCode.F));
        inputSkip = true;
    }
}