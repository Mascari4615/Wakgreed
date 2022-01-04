using System.Collections;
using UnityEngine;
using FMODUnity;
using Cinemachine;
using TMPro;

public class Architect : InteractiveObject
{
    [TextArea][SerializeField] private string comment;
    [SerializeField] private BoolVariable isShowingSomething;
    private CinemachineVirtualCamera cvm1, cvm2; // NPC, UI
    private GameObject customUI, chat;
    private TextMeshProUGUI chatText;
    private bool isTalking, inputSkip;
    private readonly WaitForSeconds ws005 = new(0.05f), ws02 = new(0.2f);

    [SerializeField] protected BuildingInventoryUI inventoryUI;
    [SerializeField] protected IntVariable goldu;

    public void ResetInven()
    {
        GameData gameData = DataManager.Instance.CurGameData;

        inventoryUI.NpcInventory.Clear();
        foreach (var building in DataManager.Instance.buildingDataBuffer.items)
        {      
            if (gameData.rescuedNPC[building.requiredNpcID] && gameData.buildedBuilding[building.id] == false)
            {
                inventoryUI.NpcInventory.Add(building);
            }
        }
    }

    protected override void Awake()
    {
        base.Awake();

        cvm1 = transform.Find("CM").GetComponent<CinemachineVirtualCamera>();
        cvm2 = transform.Find("CM2").GetComponent<CinemachineVirtualCamera>();
        customUI = transform.Find("CustomUI").gameObject;
        chat = transform.Find("DefaultUI").transform.Find("Chat").gameObject;
        chatText = chat.transform.GetChild(1).GetChild(2).GetComponent<TextMeshProUGUI>();
        cvm1.Follow = GameObject.Find("Cameras").transform.GetChild(2);

        ResetInven();
    }

    public void BuildBuilding(Slot slot)
    {
        if (goldu.RuntimeValue >= (slot.SpecialThing as Building).price)
        {
            goldu.RuntimeValue -= (slot.SpecialThing as Building).price;

            slot.gameObject.SetActive(false);

            inventoryUI.NpcInventory.Remove(slot.SpecialThing as Building);
            DataManager.Instance.CurGameData.buildedBuilding[(slot.SpecialThing as Building).id] = true;
            Lobby.instance.ResetLobby();
            RuntimeManager.PlayOneShot($"event:/SFX/UI/Test", transform.position);
            inventoryUI.Initialize();
        }
        else
        {
            RuntimeManager.PlayOneShot($"event:/SFX/UI/No", transform.position);
            ObjectManager.Instance.PopObject("AnimatedText", Wakgood.Instance.transform.position).GetComponent<AnimatedText>().SetText("∞ÒµŒ ∫Œ¡∑!", Color.yellow);
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