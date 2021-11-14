using FMODUnity;
using System;
using System.Collections;
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
    [SerializeField] private GameObject textGameObjectPrefab;
    [SerializeField] private TMP_InputField inputField;
    private float t = 0;

    [SerializeField] private Connect twitchConnect;

    
    private void Awake()
    {
        Instance = this;
        
        for (int i = 1; i < chatPanel.transform.childCount; i++)
        {
            Destroy(chatPanel.transform.GetChild(i).gameObject);
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
                donationUI.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = $"Ttmdacl님 꼐서 {donationAmount}골드 조공";
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
            // chatGameObject.SetActive(false);
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
        // Log log = new Log();

        // 대충 명령어인지 일반 채팅인지 로그인지 판독하는 코드
        //ObjectManager.Instance.GetQueue("Chat", chatPanel.transform, true).GetComponent<TextMeshProUGUI>().text = "<b>우왁굳</b> : " + msg;
        Instantiate(textGameObjectPrefab, chatPanel.transform).GetComponent<TextMeshProUGUI>().text = "<b>우왁굳</b> : " + msg;
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
}