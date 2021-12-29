using Cinemachine;
using FMODUnity;
using System.Collections;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using TMPro;
using UnityEngine;

public class Ttmdacl : InteractiveObject
{
    [TextArea][SerializeField] private string comment;
    [SerializeField] private BoolVariable isShowingSomething;
    private CinemachineVirtualCamera cvm1, cvm2; // NPC, UI
    [SerializeField] private GameObject loadButton, mark;
    private GameObject customUI, chat;
    private TextMeshProUGUI chatText;
    private bool isTalking, inputSkip;
    private readonly WaitForSeconds ws005 = new(0.05f), ws02 = new(0.2f);

    protected override void Awake()
    {
        base.Awake();

        cvm1 = transform.Find("CM").GetComponent<CinemachineVirtualCamera>();
        cvm2 = transform.Find("CM2").GetComponent<CinemachineVirtualCamera>();
        customUI = transform.Find("CustomUI").gameObject;
        chat = transform.Find("DefaultUI").transform.Find("Chat").gameObject;
        chatText = chat.transform.GetChild(1).GetChild(2).GetComponent<TextMeshProUGUI>();
        cvm1.Follow = GameObject.Find("Cameras").transform.GetChild(2);
    }

    private void OnEnable()
    {
        if (File.Exists(Path.Combine(Application.streamingAssetsPath, "gameSave.wak")))
        {
            mark.SetActive(true);
            loadButton.SetActive(true);
        }
        else
        {
            mark.SetActive(false);
            loadButton.SetActive(false);
        }
    }

    public void LoadGame()
    {
        BinaryFormatter bf = new();
        FileStream stream = new(Path.Combine(Application.streamingAssetsPath, "gameSave.wak"), FileMode.Open);
        GameData2 data2 = bf.Deserialize(stream) as GameData2;
        stream.Close();

        StartCoroutine(GameManager.Instance.EnterPortal(data2));
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

        chat.SetActive(true);

        chatText.text = "";

        IEnumerator checkSkip;
        StartCoroutine(checkSkip = CheckSkipInput());
        foreach (char item in comment)
        {
            if (!inputSkip)
            {
                chatText.text += item;
                RuntimeManager.PlayOneShot($"event:/SFX/NPC/NPC_Temp", transform.position);
                yield return ws005;
            }
            else
            {
                chatText.text = comment;
                break;
            }
        }
        StopCoroutine(checkSkip);
        inputSkip = false;

        yield return ws02;

        do yield return null;
        while (!Input.GetKeyDown(KeyCode.F));

        chat.SetActive(false);

        GameManager.Instance.CinemachineTargetGroup.m_Targets[1].target = null;
        GameManager.Instance.CinemachineTargetGroup.m_Targets[1].weight = 1;
        cvm1.Priority = -100;
        isTalking = false;
        customUI.SetActive(true);
        cvm2.Priority = 300;
    }

    private IEnumerator CheckSkipInput()
    {
        do yield return null;
        while (!Input.GetKeyDown(KeyCode.F));
        inputSkip = true;
    }
}