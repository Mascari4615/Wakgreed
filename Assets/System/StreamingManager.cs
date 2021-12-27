using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class StreamingManager : MonoBehaviour
{
    public static StreamingManager Instance { get; private set; }
    
    [SerializeField] private TMP_InputField inputField; 
    [SerializeField] private TextMeshProUGUI donationText, upTimeUI;
    [SerializeField] private GameObject donationUI, chatViewport, chatContent;
    [SerializeField] private Image donationImageUI;
    [SerializeField] private Sprite[] donationImages;
    [SerializeField] private IntVariable goldu, viewer;
    [SerializeField] private BoolVariable isChatting, isLoading;
    [SerializeField] private TwitchConnect twitchConnect;

    [SerializeField] private Buff[] buffs;

    private float t;
    private readonly List<TextMeshProUGUI> chatPool = new();
    private int chatIndex;
    private Coroutine showWakgoodChat;
    private bool isStreaming;
    private readonly WaitForSeconds ws15 = new(15f), ws05 = new(.5f), ws02 = new(0.2f);
    public string Uptime { get; private set; } = "";

    private void Awake()
    {
        Instance = this;

        for (int i = 0; i < chatContent.transform.childCount; i++)
            chatPool.Add(chatContent.transform.GetChild(i).GetComponent<TextMeshProUGUI>()); 
    }

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(1f);
        twitchConnect.ConnectToTwitch();
    }

    public void StartStreaming()
    {
        if (isStreaming) return;
        Debug.Log("BangOn");

        isStreaming = true;
        viewer.RuntimeValue = 3000;

        // GameEventListener 클래스를 통해 꼼수로 코루틴 실행하기
        if (DataManager.Instance.CurGameData.rescuedNPC[16])
            StartCoroutine(Donation_Dandab());

        if (DataManager.Instance.CurGameData.rescuedNPC[2])
            StartCoroutine(Donation_R());

        if (DataManager.Instance.CurGameData.rescuedNPC[2])
            StartCoroutine(Donation_Secret());

        StartCoroutine(CheckViewer());
        StartCoroutine(UpdateUptime());
    }

    public void BangJong()
    {
        // GameEventListener 클래스를 통해 꼼수로 코루틴 종료하기 (StopCoroutine 실행)

        Debug.Log("BangJong");
        isStreaming = false;
        StopAllCoroutines();
    }

    private IEnumerator Donation_Secret()
    {
        yield return new WaitForSeconds(18f);

        while (true)
        {
            while (isLoading.RuntimeValue) yield return null;

            if (Random.Range(0, 100) < 30)
            {
                Wakgood.Instance.ReceiveHeal(1);
                donationUI.SetActive(false);
                donationText.text = $"비밀소녀의 응원으로 체력 1 회복!";
                donationImageUI.sprite = donationImages[2];
                donationUI.SetActive(true);
                RuntimeManager.PlayOneShot($"event:/SFX/ETC/Donation");
            }
            yield return new WaitForSeconds(20f);
        }
    }

    private IEnumerator Donation_R()
    {
        yield return new WaitForSeconds(4f);

        while (true)
        {
            while (isLoading.RuntimeValue) yield return null;

            if (Random.Range(0, 100) < 70)
            {
                DataManager.Instance.buffRunTimeSet.Add(buffs[0]);
                donationUI.SetActive(false);
                donationText.text = $"15초 동안 공격속도 30% 증가!";
                donationImageUI.sprite = donationImages[0];
                donationUI.SetActive(true);
                RuntimeManager.PlayOneShot($"event:/SFX/ETC/Donation");
            }
            yield return new WaitForSeconds(30f);
        }
    }

    private IEnumerator Donation_Dandab()
    {
        yield return new WaitForSeconds(15f);
        while (true)
        {
            while (isLoading.RuntimeValue) yield return null;

            if (Random.Range(0, 100) < 30)
            {
                DataManager.Instance.buffRunTimeSet.Add(buffs[1]);
                donationUI.SetActive(false);
                donationText.text = $"10초 동안 회피율 30% 증가!";
                donationImageUI.sprite = donationImages[1];
                donationUI.SetActive(true);
                RuntimeManager.PlayOneShot($"event:/SFX/ETC/Donation");
            }
            yield return new WaitForSeconds(17f);
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

            Uptime = $"{dt:HH:mm:ss}";
            upTimeUI.SetText(Uptime);
            yield return ws02;
        }
    }

    public void GetViewer(int amount) => viewer.RuntimeValue += amount;

    private void Update()
    {
        isChatting.RuntimeValue = inputField.gameObject.activeSelf;

        if (Input.GetKey(KeyCode.Z) && !chatViewport.activeSelf) chatViewport.SetActive(true);
        else if (Input.GetKeyUp(KeyCode.Z)) t = 2;

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (inputField.gameObject.activeSelf == false)
            {
                chatViewport.SetActive(true);
                inputField.gameObject.SetActive(true);
                inputField.ActivateInputField();
            }
            else if (inputField.text != "")
            { 
                Chat(inputField.text);
                inputField.text = "";
                inputField.gameObject.SetActive(false);
            }
        }

        if (Input.GetKeyDown(KeyCode.Slash) && inputField.gameObject.activeSelf == false)
        {
            chatViewport.SetActive(true);
            inputField.gameObject.SetActive(true);
            inputField.ActivateInputField();
            inputField.text = "/";
            inputField.stringPosition = 10;            
        }

        if (chatViewport.activeSelf && !inputField.gameObject.activeSelf && !Input.GetKey(KeyCode.Z))
        {
            t -= Time.deltaTime;
            if (t <= 0) chatViewport.SetActive(false);
        }
    }

    public void Chat(string msg, string nickName = "우왁굳")
    {
        if (msg.StartsWith('/'))
        {
            if (msg.Split(' ').Length == 0) return;

            switch (msg[1..msg.IndexOf(' ')])
            {
                case "item":
                case "아이템":
                    for (int i = 0; i < (msg.Split(' ').Length == 3 ? int.Parse(msg.Split(' ')[2]) : 1); i++)
                        DebugManager.GetItem(int.Parse(msg.Split(' ')[1]));
                    break;
                case "weapon":
                case "무기":
                    DebugManager.GetWeapon(int.Parse(msg.Split(' ')[1]));
                    break;
                case "food":
                case "음식":
                    DataManager.Instance.wakgoodFoodInventory.Add(DataManager.Instance.FoodDic[int.Parse(msg.Split(' ')[1])]);
                    break;
                default:
                    if (msg[1..(msg.Length - 1)] == "clear" || msg[1..(msg.Length - 1)] == "초기화")
                    {
                        DataManager.Instance.wakgoodItemInventory.Clear();
                        DataManager.Instance.wakgoodFoodInventory.Clear();
                        DataManager.Instance.wakgoodMasteryInventory.Clear();
                        DataManager.Instance.buffRunTimeSet.Clear();
                    }
                    Debug.Log("Nope");
                    break;
            }

            return;
        }

        if (chatViewport.activeSelf == false) chatViewport.SetActive(true);

        if (nickName == "우왁굳")
        {
            if (showWakgoodChat != null) StopCoroutine(showWakgoodChat);
            showWakgoodChat = StartCoroutine(Wakgood.Instance.ShowChat(msg));
            // twitchConnect.SendMassage();
        }

        chatPool[chatIndex % 25].text = $"<b><color=#{ColorUtility.ToHtmlStringRGBA(Random.ColorHSV())}>{nickName}</color></b>\n{msg}";

        chatContent.transform.GetChild(0).transform.SetAsLastSibling();
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