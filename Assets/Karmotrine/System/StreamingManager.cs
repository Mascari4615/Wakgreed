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
    [SerializeField] private TMP_InputField inputField;
    public TextMeshProUGUI[] donationText;
    [SerializeField] private TextMeshProUGUI upTimeUI;
    public GameObject[] donationUI;
    [SerializeField] private GameObject viewPort, chatContent;
    public Image[] donationImageUI;
    public Sprite[] donationImages;
    [SerializeField] private IntVariable goldu, viewer;
    [SerializeField] private BoolVariable isLoading;
    [SerializeField] private IntVariable uMin, uMax;
    [SerializeField] private IntVariable gMin, gMax;

    [SerializeField] private Buff[] buffs;

    public bool temp;
    private readonly List<TextMeshProUGUI> chatPool = new();
    private readonly WaitForSeconds ws15 = new(15f), ws1 = new(1f), ws02 = new(0.2f);
    private int chatIndex;
    private bool isStreaming;
    private Coroutine showWakgoodChat;

    private float t;
    // [SerializeField] private TwitchConnect twitchConnect;

    public static StreamingManager Instance { get; private set; }

    public bool IsChatting => inputField.gameObject.activeSelf;
    public string Uptime { get; private set; } = "";

    private void Awake()
    {
        Instance = this;

        for (int i = 0; i < chatContent.transform.childCount; i++)
        {
            chatPool.Add(chatContent.transform.GetChild(i).GetComponent<TextMeshProUGUI>());
        }
    }

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(1f);
        // twitchConnect.ConnectToTwitch();
    }

    private void Update()
    {
        if (viewPort.activeSelf && !inputField.gameObject.activeSelf && !Input.GetKey(KeyCode.Z))
        {
            t -= Time.deltaTime;
            if (t <= 0)
            {
                viewPort.SetActive(false);
            }
        }

        if (Input.GetKey(KeyCode.Z) && !viewPort.activeSelf)
        {
            viewPort.SetActive(true);
        }
        else if (Input.GetKeyUp(KeyCode.Z))
        {
            t = 2;
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (inputField.gameObject.activeSelf == false)
            {
                viewPort.SetActive(true);
                inputField.gameObject.SetActive(true);
                inputField.ActivateInputField();
            }
            else if (inputField.text != "")
            {
                Chat(inputField.text);
                inputField.text = "";
                inputField.gameObject.SetActive(false);
            }
            else
            {
                inputField.gameObject.SetActive(false);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Slash) && inputField.gameObject.activeSelf == false)
        {
            viewPort.SetActive(true);
            inputField.gameObject.SetActive(true);
            inputField.ActivateInputField();
            inputField.text = "/";
            inputField.stringPosition = 10;
        }
    }

    public void StartStreaming()
    {
        if (isStreaming)
        {
            return;
        }

        isStreaming = true;

        if (DataManager.Instance.CurGameData.rescuedNPC[18])
        {
            DataManager.Instance.wgItemInven.Add(DataManager.Instance.ItemDic[12]);
        }

        if (DataManager.Instance.CurGameData.rescuedNPC[13])
        {
            DataManager.Instance.wgItemInven.Add(DataManager.Instance.ItemDic[39]);
        }

        if (DataManager.Instance.CurGameData.rescuedNPC[7])
        {
            if (Random.Range(0, 1 + 1) == 1)
            {
                DataManager.Instance.wgItemInven.Add(DataManager.Instance.ItemDic[27]);
            }
            else
            {
                DataManager.Instance.wgItemInven.Add(DataManager.Instance.ItemDic[44]);
            }
        }

        // GameEventListener 클래스를 통해 꼼수로 코루틴 실행하기
        if (DataManager.Instance.CurGameData.rescuedNPC[22] ||
            DataManager.Instance.CurGameData.rescuedNPC[23] ||
            DataManager.Instance.CurGameData.rescuedNPC[24] ||
            DataManager.Instance.CurGameData.rescuedNPC[25] ||
            DataManager.Instance.CurGameData.rescuedNPC[26] ||
            DataManager.Instance.CurGameData.rescuedNPC[27])
        {
            StartCoroutine(Hosting());
        }

        if (DataManager.Instance.CurGameData.rescuedNPC[16])
        {
            StartCoroutine(Donation_Dandab());
        }

        if (DataManager.Instance.CurGameData.rescuedNPC[3])
        {
            StartCoroutine(Donation_R());
        }

        if (DataManager.Instance.CurGameData.rescuedNPC[2])
        {
            StartCoroutine(Donation_Secret());
        }

        StartCoroutine(CheckViewer());
        StartCoroutine(UpdateUptime());
    }

    public void BangJong()
    {
        // GameEventListener 클래스를 통해 꼼수로 코루틴 종료하기 (StopCoroutine 실행)
        isStreaming = false;
        StopAllCoroutines();
    }

    private IEnumerator Hosting()
    {
        yield return new WaitForSeconds(60f);

        while (true)
        {
            while (isLoading.RuntimeValue)
            {
                yield return null;
            }

            int temp = 0;
            if (DataManager.Instance.CurGameData.rescuedNPC[22])
            {
                temp++;
            }

            if (DataManager.Instance.CurGameData.rescuedNPC[23])
            {
                temp++;
            }

            if (DataManager.Instance.CurGameData.rescuedNPC[24])
            {
                temp++;
            }

            if (DataManager.Instance.CurGameData.rescuedNPC[25])
            {
                temp++;
            }

            if (DataManager.Instance.CurGameData.rescuedNPC[26])
            {
                temp++;
            }

            if (DataManager.Instance.CurGameData.rescuedNPC[27])
            {
                temp++;
            }

            if (GameManager.Instance.isRealBossing.RuntimeValue && GameManager.Instance.isRealBossFirstDeath)
            {
            }
            else
            {
                if (Random.Range(0, 100) < temp * 3)
                {
                    int amount = Random.Range(100, 100 * temp);
                    viewer.RuntimeValue += amount;
                    donationUI[0].SetActive(false);
                    donationText[0].text = $"이세계 아이돌이 {amount}명 호스팅!";
                    donationImageUI[0].sprite = donationImages[3];
                    donationUI[0].SetActive(true);
                    RuntimeManager.PlayOneShot("event:/SFX/ETC/Donation");
                }


                yield return new WaitForSeconds(142f);
            }
        }
    }

    private IEnumerator Donation_Secret()
    {
        yield return new WaitForSeconds(18f);

        while (true)
        {
            while (isLoading.RuntimeValue)
            {
                yield return null;
            }

            int temp = 0;
            if (DataManager.Instance.wgItemInven.Items.Contains(DataManager.Instance.ItemDic[48]))
            {
                temp = DataManager.Instance.wgItemInven.itemCountDic[48];
            }

            if (GameManager.Instance.isRealBossing.RuntimeValue && GameManager.Instance.isRealBossFirstDeath)
            {
            }
            else if (Random.Range(0, 100) < 30)
            {
                Wakgood.Instance.ReceiveHeal(1 + temp);
                donationUI[0].SetActive(false);
                donationText[0].text = $"비밀소녀의 응원으로 체력 {1 + temp} 회복!";
                donationImageUI[0].sprite = donationImages[2];
                donationUI[0].SetActive(true);
                RuntimeManager.PlayOneShot("event:/SFX/ETC/Donation");
            }


            yield return new WaitForSeconds(47f);
        }
    }

    private IEnumerator Donation_R()
    {
        yield return new WaitForSeconds(4f);

        while (true)
        {
            while (isLoading.RuntimeValue)
            {
                yield return null;
            }

            int temp = 0;
            if (DataManager.Instance.wgItemInven.Items.Contains(DataManager.Instance.ItemDic[47]))
            {
                temp = DataManager.Instance.wgItemInven.itemCountDic[47];
            }

            if (GameManager.Instance.isRealBossing.RuntimeValue && GameManager.Instance.isRealBossFirstDeath)
            {
            }
            else if (Random.Range(0, 100) < 50 + (temp * 5))
            {
                DataManager.Instance.buffRunTimeSet.Add(buffs[0]);
                donationUI[0].SetActive(false);
                donationText[0].text = "10초 동안 공격속도 20% 증가!";
                donationImageUI[0].sprite = donationImages[0];
                donationUI[0].SetActive(true);
                RuntimeManager.PlayOneShot("event:/SFX/ETC/Donation");
            }


            yield return new WaitForSeconds(64f);
        }
    }

    private IEnumerator Donation_Dandab()
    {
        yield return new WaitForSeconds(15f);
        while (true)
        {
            while (isLoading.RuntimeValue)
            {
                yield return null;
            }

            if (GameManager.Instance.isRealBossing.RuntimeValue && GameManager.Instance.isRealBossFirstDeath)
            {
            }
            else if (Random.Range(0, 100) < 30)
            {
                DataManager.Instance.buffRunTimeSet.Add(buffs[1]);
                donationUI[0].SetActive(false);
                donationText[0].text = "10초 동안 회피율 30% 증가!";
                donationImageUI[0].sprite = donationImages[1];
                donationUI[0].SetActive(true);
                RuntimeManager.PlayOneShot("event:/SFX/ETC/Donation");
            }

            yield return new WaitForSeconds(17f);
        }
    }

    public IEnumerator CheckViewer()
    {
        while (true)
        {
            while (viewer.RuntimeValue > 0)
            {
                while (isLoading.RuntimeValue)
                {
                    yield return null;
                }

                viewer.RuntimeValue += Random.Range(uMin.RuntimeValue, uMax.RuntimeValue + 1);
                viewer.RuntimeValue =
                    Mathf.Clamp(
                        viewer.RuntimeValue + Random.Range(-1 * gMax.RuntimeValue, (-1 * gMin.RuntimeValue) + 1), 0,
                        int.MaxValue);

                if (viewer.RuntimeValue <= 0)
                {
                    viewer.RuntimeValue = 0;
                    break;
                }

                yield return ws1;
            }

            if (GameManager.Instance.isRealBossing.RuntimeValue && GameManager.Instance.isRealBossFirstDeath)
            {
                if (temp == false)
                {
                    Debug.Log("asd");
                    Wakgood.Instance.FakeCollapse();
                    temp = true;
                }
            }
            else
            {
                DataManager.Instance.CurGameData.deathCount++;
                DataManager.Instance.SaveGameData();
                Wakgood.Instance.Collapse();
                break;
            }

            Debug.Log("??");
            yield return null;
        }
    }

    private IEnumerator UpdateUptime()
    {
        float startTime = Time.time;

        while (true)
        {
            while (isLoading.RuntimeValue)
            {
                yield return null;
            }

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

    public void GetViewer(int amount)
    {
        viewer.RuntimeValue += amount;
    }

    public void Chat(string msg, string nickName = "우왁굳")
    {
        if (msg.StartsWith('/'))
        {
            if (msg.Split(' ').Length == 0)
            {
                return;
            }

            switch (msg[1..msg.IndexOf(' ')])
            {
                case "item":
                case "아이템":
                    for (int i = 0; i < (msg.Split(' ').Length == 3 ? int.Parse(msg.Split(' ')[2]) : 1); i++)
                    {
                        DataManager.Instance.wgItemInven.Add(
                            DataManager.Instance.ItemDic[int.Parse(msg.Split(' ')[1])]);
                    }

                    break;
                case "weapon":
                case "무기":
                    Wakgood.Instance.SwitchWeapon(Wakgood.Instance.CurWeaponNumber,
                        DataManager.Instance.WeaponDic[int.Parse(msg.Split(' ')[1])]);
                    break;
                case "food":
                case "음식":
                    DataManager.Instance.wgFoodInven.Add(DataManager.Instance.FoodDic[int.Parse(msg.Split(' ')[1])]);
                    break;
                case "골두":
                    goldu.RuntimeValue += int.Parse(msg.Split(' ')[1]);
                    break;
                default:
                    if (msg[1..(msg.Length - 1)] == "clear" || msg[1..(msg.Length - 1)] == "초기화")
                    {
                        DataManager.Instance.wgItemInven.Clear();
                        DataManager.Instance.wgFoodInven.Clear();
                        DataManager.Instance.buffRunTimeSet.Clear();
                    }

                    Debug.Log("Nope");
                    break;
            }

            return;
        }

        if (viewPort.activeSelf == false)
        {
            viewPort.SetActive(true);
        }

        if (nickName == "우왁굳")
        {
            if (showWakgoodChat != null)
            {
                StopCoroutine(showWakgoodChat);
            }

            showWakgoodChat = StartCoroutine(Wakgood.Instance.ShowChat(msg));
            // twitchConnect.SendMassage();
        }

        chatPool[chatIndex % 25].text =
            $"<b><color=#{ColorUtility.ToHtmlStringRGBA(Random.ColorHSV())}>{nickName}</color></b>\n{msg}";

        chatContent.transform.GetChild(0).transform.SetAsLastSibling();
        chatIndex++;

        t = 5;
    }

    public bool Temp()
    {
        if (inputField.gameObject.activeSelf)
        {
            inputField.text = "";
            inputField.gameObject.SetActive(false);
            t = 5;
            return true;
        }

        return false;
    }

    // private void OnApplicationQuit() => twitchConnect.LeaveChannel();
}