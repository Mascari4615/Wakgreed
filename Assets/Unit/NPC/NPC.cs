using Cinemachine;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class NPC : InteractiveObject
{
    public int ID;
    [TextArea] [SerializeField] private List<string> firstComment, randomComment;
    [SerializeField] protected bool canOpenUI = true;
    [SerializeField] private BoolVariable isShowingSomething;
    protected CinemachineVirtualCamera cvm1, cvm2; // NPC, UI
    protected GameObject customUI, defaultUI, chat;
    private TextMeshProUGUI chatText;
    private bool isTalking, inputSkip;
    private readonly WaitForSeconds ws005 = new(0.05f), ws02 = new(0.2f);
    protected Animator animator;

    protected override void Awake()
    {
        base.Awake();

        cvm1 = transform.Find("CM").GetComponent<CinemachineVirtualCamera>();
        cvm2 = transform.Find("CM2").GetComponent<CinemachineVirtualCamera>();
        customUI = transform.Find("CustomUI").gameObject;
        defaultUI = transform.Find("DefaultUI").gameObject;
        chat = defaultUI.transform.Find("Chat").gameObject;
        chatText = chat.transform.GetChild(1).GetChild(2).GetComponent<TextMeshProUGUI>();
        cvm1.Follow = GameObject.Find("Cameras").transform.GetChild(2);
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

    protected override void OnTriggerExit2D(Collider2D other)
    {
        base.OnTriggerExit2D(other);

        if (!other.CompareTag("Player"))
        {
            return;
        }
        
        GameManager.Instance.CinemachineTargetGroup.m_Targets[1].target = null;
        GameManager.Instance.CinemachineTargetGroup.m_Targets[1].weight = 1;
        cvm1.Priority = -100;
        cvm2.Priority = -100;
        customUI.SetActive(false);
        chat.SetActive(false);

        StopAllCoroutines();
        isTalking = false;
        inputSkip = false;
        isShowingSomething.RuntimeValue = false;
    }

    public virtual void FocusOff()
    {
        GameManager.Instance.CinemachineTargetGroup.m_Targets[1].target = null;
        GameManager.Instance.CinemachineTargetGroup.m_Targets[1].weight = 1;
        cvm1.Priority = -100;
        cvm2.Priority = -100;
        customUI.SetActive(false);
        chat.SetActive(false);

        StopAllCoroutines();
        isTalking = false;
        inputSkip = false;
        isShowingSomething.RuntimeValue = false;
    }

    private IEnumerator OnType()
    {
        GameManager.Instance.CinemachineTargetGroup.m_Targets[1].target = transform;
        GameManager.Instance.CinemachineTargetGroup.m_Targets[1].weight = 5;
        cvm1.Priority = 200;

        List<string> tempComment;

        if (DataManager.Instance.CurGameData.rescuedNPC[ID] == false)
        {
            tempComment = firstComment;
        }
        else if (DataManager.Instance.CurGameData.talkedOnceNPC[ID] == false)
        {
            DataManager.Instance.CurGameData.talkedOnceNPC[ID] = true;
            DataManager.Instance.SaveGameData();
            tempComment = firstComment;
        }
        else
        {
            tempComment = new List<string>(1)
            {
                randomComment[Random.Range(0, randomComment.Count)]
            };
        }

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
                    RuntimeManager.PlayOneShot($"event:/SFX/NPC/NPC_{ID}", transform.position);
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

        GameManager.Instance.CinemachineTargetGroup.m_Targets[1].target = null;
        GameManager.Instance.CinemachineTargetGroup.m_Targets[1].weight = 1;
        cvm1.Priority = -100;
        isTalking = false;
        
        if (canOpenUI)
        {
            customUI.SetActive(true);
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