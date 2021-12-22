using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class StreamingManager : MonoBehaviour
{
    public static StreamingManager Instance { get; private set; }
    
    [SerializeField] private TMP_InputField inputField; 
    [SerializeField] private TextMeshProUGUI donationText, upTimeUI;
    [SerializeField] private GameObject donationUI, chatGameObject, chatPanel;
    [SerializeField] private IntVariable goldu, viewer;
    [SerializeField] private BoolVariable isChatting, isLoading;
    [SerializeField] private TwitchConnect twitchConnect;

    private float t;
    private readonly List<TextMeshProUGUI> chatPool = new();
    private int chatIndex;
    private Coroutine showWakgoodChat;
    private bool isStreaming;
    private readonly WaitForSeconds ws5 = new(5f), ws05 = new(.5f), ws02 = new(0.2f);
    
    private void Awake()
    {
        Instance = this;  
        twitchConnect.ConnectToTwitch();

        for (int i = 0; i < chatPanel.transform.childCount; i++)
            chatPool.Add(chatPanel.transform.GetChild(i).GetComponent<TextMeshProUGUI>());
    }

    public void StartStreaming()
    {
        if (isStreaming) return;

        isStreaming = true;
        viewer.RuntimeValue = 3000;

        // GameEventListener 클래스를 통해 꼼수로 코루틴 실행하기
        StartCoroutine(GetDonation());
        StartCoroutine(CheckViewer());
        StartCoroutine(UpdateUptime());
    }

    public void BangJong()
    {
        isStreaming = false;
        StopAllCoroutines();
    }

    private IEnumerator GetDonation()
    {
        // GameEventListener 클래스를 통해 꼼수로 코루틴 종료하기 (StopCoroutine 실행)
        while (true)
        {
            while (isLoading.RuntimeValue) yield return null;
            
            viewer.RuntimeValue -= Random.Range(1, 10 + 1);

            if (Random.Range(0, 100) < 30)
            {
                int donationAmount = Random.Range(500, 1001);
                goldu.RuntimeValue += donationAmount;

                // 꼼수로 애니메이션 실행하기
                // # UI를 분리시키는 작업 필요
                donationUI.SetActive(false);
                donationText.text = $"@@님께서 {donationAmount}골두 조공!";
                donationUI.SetActive(true);

                RuntimeManager.PlayOneShot($"event:/SFX/ETC/Donation");
            }

            yield return ws5;
        }
    }

    private IEnumerator CheckViewer()
    {
        while (viewer.RuntimeValue > 0)
        {
            while (isLoading.RuntimeValue) yield return null;
            
            viewer.RuntimeValue += Random.Range(-3, 1 + 1);
            yield return ws05;
        }

        Debug.Log("!");
    }

    private IEnumerator UpdateUptime()
    {
        float startTime = Time.time;

        while (true)
        {
            while (isLoading.RuntimeValue) yield return null;
            
            int curTime = (int)(Time.time - startTime);
            int hour = curTime / 3600;
            int minute = (curTime -= hour * 3600) / 60;
            int second = (curTime -= hour * 3600) % 60;
            DateTime dt = new(1987, 7, 24, hour, minute, second);

            upTimeUI.SetText($"{dt:HH:mm:ss}");
            yield return ws02;
        }
    }

    public void GetViewer(int amount) => viewer.RuntimeValue += amount;

    private void Update()
    {
        isChatting.RuntimeValue = inputField.gameObject.activeSelf;

        if (Input.GetKey(KeyCode.Z) && !chatGameObject.activeSelf) chatGameObject.SetActive(true);
        else if (Input.GetKeyUp(KeyCode.Z)) t = 2;

        if (Input.GetKeyDown(KeyCode.Return))
        {
            inputField.gameObject.SetActive(!inputField.gameObject.activeSelf);

            if (inputField.gameObject.activeSelf == false)
            {
                chatGameObject.SetActive(true);
                inputField.ActivateInputField();
            }
            else if (inputField.text != "")
            { 
                Chat(inputField.text);
                inputField.text = "";
            }
        }

        if (Input.GetKeyDown(KeyCode.Slash) && inputField.gameObject.activeSelf == false)
        {
            chatGameObject.SetActive(true);
            inputField.gameObject.SetActive(true);
            inputField.ActivateInputField();
            inputField.text = "/";
            inputField.stringPosition = 10;            
        }

        if (chatGameObject.activeSelf && !inputField.gameObject.activeSelf && !Input.GetKey(KeyCode.Z))
        {
            t -= Time.deltaTime;
            if (t <= 0) chatGameObject.SetActive(false);
        }
    }

    public void Chat(string msg, string nickName = "우왁굳")
    {
        if (msg.StartsWith('/'))
        {
            switch (msg[1..msg.IndexOf(' ')])
            {
                case "item":
                case "아이템":
                    for (int i = 0; i < (msg.Split(' ').Length == 3 ? int.Parse(msg.Split(' ')[2]) : 0); i++)
                        DebugManager.GetItem(int.Parse(msg.Split(' ')[1]));
                    break;
                case "weapon":
                case "무기":
                    DebugManager.GetWeapon(int.Parse(msg.Split(' ')[1]));
                    break;
                case "food":
                case "음식":
                    DataManager.Instance.wakgoodFoodInventory.Add(DataManager.Instance.FoodDic[int.Parse(msg.Split(' ')[1])]);
                    break;;
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

            return;
        }

        if (chatGameObject.activeSelf == false) chatGameObject.SetActive(true);

        if (nickName == "우왁굳")
        {
            if (showWakgoodChat != null) StopCoroutine(showWakgoodChat);
            showWakgoodChat = StartCoroutine(Wakgood.Instance.ShowChat(msg));
            // twitchConnect.SendMassage();
        }

        chatPool[chatIndex % 25].text = $"<b>{nickName}</b> : {msg}";
        chatPanel.transform.GetChild(0).transform.SetAsLastSibling();
        chatIndex++;

        t = 5;
    }

    public bool Temp()
    {
        if (inputField.gameObject.activeSelf == true)
        {
            inputField.text = "";
            inputField.gameObject.SetActive(false);
            t = 5;
            return true;
        }
        else return false;
    }

    private void OnApplicationQuit() => twitchConnect.LeaveChannel();
}