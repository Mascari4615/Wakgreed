using Cinemachine;
using FMODUnity;
using System.Collections;
using TMPro;
using UnityEngine;

public class MasteryMaster : InteractiveObject
{
    [TextArea] [SerializeField] private string comment;
    [SerializeField] private BoolVariable isShowingSomething;

    [SerializeField] private TextMeshProUGUI[] masteryStackTexts;
    [SerializeField] private GameObject[] stamps;
    [SerializeField] private TextMeshProUGUI remainMasteryStackText;
    private readonly WaitForSeconds ws005 = new(0.05f), ws02 = new(0.2f);
    private TextMeshProUGUI chatText;
    private GameObject customUI, chat;
    private CinemachineVirtualCamera cvm1, cvm2; // NPC, UI
    private bool isTalking, inputSkip;

    protected override void Awake()
    {
        base.Awake();
        cvm1 = transform.Find("CM").GetComponent<CinemachineVirtualCamera>();
        cvm2 = transform.Find("CM2").GetComponent<CinemachineVirtualCamera>();
        customUI = transform.Find("CustomUI").gameObject;
        chat = transform.Find("DefaultUI").transform.Find("Chat").gameObject;
        chatText = chat.transform.GetChild(1).GetChild(2).GetComponent<TextMeshProUGUI>();
        cvm1.Follow = GameObject.Find("Cameras").transform.GetChild(2);
        ResetUI();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && customUI.activeSelf)
        {
            ResetStack();
        }
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

    private void ResetStack()
    {
        DataManager.Instance.wgMasteryInven.Clear();
        for (int i = 0; i < masteryStackTexts.Length; i++)
        {
            DataManager.Instance.CurGameData.masteryStacks[i] = 0;
            stamps[i * 3].SetActive(false);
            stamps[(i * 3) + 1].SetActive(false);
            stamps[(i * 3) + 2].SetActive(false);
            masteryStackTexts[i].text = "0";
        }

        DataManager.Instance.CurGameData.masteryStack = DataManager.Instance.CurGameData.level;
        remainMasteryStackText.text = DataManager.Instance.CurGameData.masteryStack.ToString();
    }

    public void ResetUI()
    {
        for (int i = 0; i < masteryStackTexts.Length; i++)
        {
            masteryStackTexts[i].text = DataManager.Instance.CurGameData.masteryStacks[i].ToString();
        }

        remainMasteryStackText.text = DataManager.Instance.CurGameData.masteryStack.ToString();

        DataManager.Instance.wgMasteryInven.Clear();
        for (int i = 0; i < masteryStackTexts.Length; i++)
        {
            if (DataManager.Instance.CurGameData.masteryStacks[i] >= 5)
            {
                DataManager.Instance.wgMasteryInven.Add(DataManager.Instance.MasteryDic[i * 3]);
                stamps[i * 3].SetActive(true);
            }
            else
            {
                stamps[i * 3].SetActive(false);
            }

            if (DataManager.Instance.CurGameData.masteryStacks[i] >= 10)
            {
                DataManager.Instance.wgMasteryInven.Add(DataManager.Instance.MasteryDic[(i * 3) + 1]);
                stamps[(i * 3) + 1].SetActive(true);
            }
            else
            {
                stamps[(i * 3) + 1].SetActive(false);
            }

            if (DataManager.Instance.CurGameData.masteryStacks[i] >= 15)
            {
                DataManager.Instance.wgMasteryInven.Add(DataManager.Instance.MasteryDic[(i * 3) + 2]);
                stamps[(i * 3) + 2].SetActive(true);
            }
            else
            {
                stamps[(i * 3) + 2].SetActive(false);
            }
        }
    }

    public void AddMasteryStack(int i)
    {
        if (DataManager.Instance.CurGameData.masteryStack > 0 && DataManager.Instance.CurGameData.masteryStacks[i] < 20)
        {
            DataManager.Instance.CurGameData.masteryStacks[i]++;
            DataManager.Instance.CurGameData.masteryStack--;
            DataManager.Instance.SaveGameData();

            if (DataManager.Instance.CurGameData.masteryStacks[i] == 5)
            {
                DataManager.Instance.wgMasteryInven.Add(DataManager.Instance.MasteryDic[i * 3]);
                stamps[i * 3].SetActive(true);
            }
            else if (DataManager.Instance.CurGameData.masteryStacks[i] == 10)
            {
                DataManager.Instance.wgMasteryInven.Add(DataManager.Instance.MasteryDic[(i * 3) + 1]);
                stamps[(i * 3) + 1].SetActive(true);
            }
            else if (DataManager.Instance.CurGameData.masteryStacks[i] == 15)
            {
                DataManager.Instance.wgMasteryInven.Add(DataManager.Instance.MasteryDic[(i * 3) + 2]);
                stamps[(i * 3) + 2].SetActive(true);
            }

            masteryStackTexts[i].text = DataManager.Instance.CurGameData.masteryStacks[i].ToString();
            remainMasteryStackText.text = DataManager.Instance.CurGameData.masteryStack.ToString();
        }
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
                RuntimeManager.PlayOneShot("event:/SFX/NPC/NPC_Temp", transform.position);
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

        do
        {
            yield return null;
        } while (!Input.GetKeyDown(KeyCode.F));

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
        do
        {
            yield return null;
        } while (!Input.GetKeyDown(KeyCode.F));

        inputSkip = true;
    }
}