using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LogManager : MonoBehaviour
{
    private static LogManager instance;
    public static LogManager Instance { get { return instance; } }

    List<Log> logList = new List<Log>();
    [SerializeField] private GameObject chatGameObject;
    public GameObject chatPanel;
    [SerializeField] private GameObject textGameObjectPrefab;
    [SerializeField] private TMP_InputField inputField;
    private float t = 0;

    [SerializeField] private Connect twitchConnect;

    private void Awake()
    {
        instance = this;

        for (int i = 1; i < chatPanel.transform.childCount; i++)
        {
            Destroy(chatPanel.transform.GetChild(i).gameObject);
            Debug.Log(i);
        }
    }
    private void Update()
    {
        if (Input.GetKey(KeyCode.Z)) { if (!chatGameObject.activeSelf) { chatGameObject.SetActive(true); } }
        else if (Input.GetKeyUp(KeyCode.Z)) t = 2; // chatGameObject.SetActive(false);

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (inputField.gameObject.activeSelf == false) { chatGameObject.SetActive(true); inputField.gameObject.SetActive(true); inputField.ActivateInputField(); }
            else { if (inputField.text != "") { Chat(inputField.text); inputField.text = ""; } inputField.gameObject.SetActive(false); t = 5; }
        }
        else if (Input.GetKeyDown(KeyCode.Slash)) 
            if (inputField.gameObject.activeSelf == false) { chatGameObject.SetActive(true); inputField.gameObject.SetActive(true); inputField.ActivateInputField(); inputField.text = "/"; inputField.stringPosition = 10; }

        if (chatGameObject.activeSelf && !inputField.gameObject.activeSelf && !Input.GetKey(KeyCode.Z))
        {
            t -= Time.deltaTime;
            if (t <= 0) chatGameObject.SetActive(false);
        }

        if (chatPanel.transform.childCount > 50)
        {

        }
    }
    public void Chat(string msg)
    {
        // Log log = new Log();

        // 대충 명령어인지 일반 채팅인지 로그인지 판독하는 코드
        ObjectManager.Instance.GetQueue("Chat", chatPanel.transform).GetComponent<TextMeshProUGUI>().text = "<b>우왁굳</b> : " + msg;
        twitchConnect.SendMassage();
        t = 5;
    }
    public void Chat(string[] msgs)
    {
        if (msgs.Length == 1) return;
        ObjectManager.Instance.GetQueue("Chat", chatPanel.transform).GetComponent<TextMeshProUGUI>().text = $"<b>{msgs[1]}</b> : {msgs[2]}";
        t = 5;
    }
    public void Log2Chat(string log)
    {

    }
}

public class Log { public string text; }
