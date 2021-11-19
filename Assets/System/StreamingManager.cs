using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class StreamingManager : MonoBehaviour
{
    public static StreamingManager Instance { get; private set; }

    [SerializeField] private GameObject donationUI;
    [SerializeField] private TextMeshProUGUI upTimeUI;
    [SerializeField] private IntVariable nyang;
    [SerializeField] private IntVariable viewer;

    [SerializeField] private GameObject chatGameObject;
    [SerializeField] private GameObject chatPanel;
    private List<TextMeshProUGUI> chatPool = new();
    private int chatIndex = 0;
    public TMP_InputField inputField;
    [SerializeField] private BoolVariable isChatting;
    public float t = 0;

    [SerializeField] private Connect twitchConnect;

    private Coroutine showWakgoodChat;


    private void Awake()
    {
        Instance = this;

        for (int i = 0; i < chatPanel.transform.childCount; i++)
        {
            chatPool.Add(chatPanel.transform.GetChild(i).GetComponent<TextMeshProUGUI>());
        }
    }

    public void StartStreaming()
    {
        // GameEventListener 클래스를 통해 꼼수로 코루틴 실행하기
        StartCoroutine(GetDonation());
        StartCoroutine(CheckViewer());
        StartCoroutine(UpdateUptime());
    }

    private IEnumerator GetDonation()
    {
        WaitForSeconds ws = new(5f);

        // GameEventListener 클래스를 통해 꼼수로 코루틴 종료하기 (StopCoroutime 실행)
        while (true)
        {
            viewer.RuntimeValue -= Random.Range(1, 10 + 1);

            if (Random.Range(0, 100) < 30)
            {
                int donationAmount = Random.Range(1, 1001);
                nyang.RuntimeValue += donationAmount;

                // 꼼수로 애니메이션 실행하기
                // # UI를 분리시키는 작업 필요
                donationUI.SetActive(false);
                donationUI.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text =
                    $"Ttmdacl님 꼐서 {donationAmount}골드 조공";
                donationUI.SetActive(true);

                RuntimeManager.PlayOneShot($"event:/SFX/ETC/Donation");
            }

            yield return ws;
        }
    }

    private IEnumerator CheckViewer()
    {
        WaitForSeconds ws = new(.5f);

        while (true)
        {
            viewer.RuntimeValue -= Random.Range(1, 3 + 1);
            yield return ws;
        }
    }

    private IEnumerator UpdateUptime()
    {
        float startTime = Time.time;
        WaitForSeconds ws02 = new WaitForSeconds(0.2f);

        while (true)
        {
            int curTime = (int)(Time.time - startTime);
            int hour = curTime / 3600;
            int minute = (curTime -= hour * 3600) / 60;
            int second = (curTime -= hour * 3600) % 60;
            DateTime dt = new DateTime(1987, 7, 24, hour, minute, second);

            upTimeUI.SetText($"{dt:HH:mm:ss}");
            yield return ws02;
        }
    }

    public void GetViewer(int amount)
    {
        viewer.RuntimeValue += amount;
    }

    private void Update()
    {
        isChatting.RuntimeValue = inputField.gameObject.activeSelf;

        if (Input.GetKey(KeyCode.Z))
        {
            if (chatGameObject.activeSelf)
            {
                return;
            }

            chatGameObject.SetActive(true);
        }
        else if (Input.GetKeyUp(KeyCode.Z))
        {
            t = 2;
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (inputField.gameObject.activeSelf == false)
            {
                chatGameObject.SetActive(true);
                inputField.gameObject.SetActive(true);
                inputField.ActivateInputField();
            }
            else
            {
                if (inputField.text != "")
                {
                    Chat(inputField.text);
                    inputField.text = "";
                }

                inputField.gameObject.SetActive(false);
                t = 5;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Slash))
        {
            if (inputField.gameObject.activeSelf == false)
            {
                chatGameObject.SetActive(true);
                inputField.gameObject.SetActive(true);
                inputField.ActivateInputField();
                inputField.text = "/";
                inputField.stringPosition = 10;
            }
        }

        if (chatGameObject.activeSelf && !inputField.gameObject.activeSelf && !Input.GetKey(KeyCode.Z))
        {
            t -= Time.deltaTime;
            if (t <= 0) chatGameObject.SetActive(false);
        }

        if (chatPanel.transform.childCount > 50)
        {
            for (int i = 0; i < chatPanel.transform.childCount - 50; i++)
            {
                //ObjectManager.Instance.InsertQueue(chatPanel.transform.GetChild(0).gameObject);
                Destroy(chatPanel.transform.GetChild(0).gameObject);
            }
        }
    }

    public void Chat(string msg)
    {
        if (msg.StartsWith('/'))
        {
            chatPool[chatIndex % 25].text = "<b>명령어</b> : " + msg;

            Debug.Log(msg.Substring(1, msg.IndexOf(' ')));
            switch (msg.Substring(1, msg.IndexOf(' ') - 1))
            {
                case "Item":
                case "item":
                case "아이템":
                    for (int i = 0; i < (msg.Split(' ').Length == 3 ? int.Parse(msg.Split(' ')[2]) : 0); i++)
                        DebugManager.GetItem(int.Parse(msg.Split(' ')[1]));
                    break;
                case "Weapon":
                case "weapon":
                case "무기":
                    DebugManager.GetWeapon(int.Parse(msg.Split(' ')[1]));
                    break;
                case "Food":
                case "food":
                case "음식":
                    DataManager.Instance.wakgoodFoodInventory.Add(DataManager.Instance.FoodDic[int.Parse(msg.Split(' ')[1])]);
                    break;;
                case "Clear":
                case "clear":
                case "초기화":
                    DataManager.Instance.wakgoodItemInventory.Clear();
                    DataManager.Instance.wakgoodFoodInventory.Clear();
                    DataManager.Instance.wakgoodMasteryInventory.Clear();
                    DataManager.Instance.buffRunTimeSet.Clear(); 
                    break;
                default:
                    Debug.Log("Nope");
                    break;
            }
        }
        else
        {
            chatPool[chatIndex % 25].text = "<b>우왁굳</b> : " + msg;

            if (showWakgoodChat != null) StopCoroutine(showWakgoodChat);
            showWakgoodChat = StartCoroutine(ShowWakgoodChat(msg));
        }

        if (chatIndex >= 25)
        {
            chatPanel.transform.GetChild(0).transform.SetAsLastSibling();
        }
        else
        {
            chatPanel.transform.GetChild(chatIndex).gameObject.SetActive(true);
        }

        chatIndex++;

        //twitchConnect.SendMassage();
        t = 5;
    }

    public void Chat(string[] msgs)
    {
        if (msgs.Length == 1) return;
        //ObjectManager.Instance.GetQueue("Chat", chatPanel.transform, true).GetComponent<TextMeshProUGUI>().text = $"<b>{msgs[1]}</b> : {msgs[2]}";
        t = 5;

        if (chatPanel.transform.childCount > 50)
        {
            for (int i = 0; i < chatPanel.transform.childCount - 50; i++)
            {
                ObjectManager.Instance.PushObject(chatPanel.transform.GetChild(0).gameObject);
            }
        }
    }

    private IEnumerator ShowWakgoodChat(string msg)
    {
        Wakgood.Instance.ChatText.text = msg;
        Wakgood.Instance.Chat.SetActive(true);
        yield return new WaitForSeconds(3f);
        Wakgood.Instance.Chat.SetActive(false);
    }
}