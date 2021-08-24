using UnityEngine;
using TMPro;
using System.Collections;
using Cinemachine;

public class NotRescuedNPC : NPC
{
    private TextMeshProUGUI text;

    [SerializeField] private float time;

    [TextArea] [SerializeField] private string comment;
    [TextArea] [SerializeField] private string comment2;

    [SerializeField] private Transform temp;

    private bool isTalking = false;
    private bool inputSkip = false;

    private void Awake()
    {
        text = ui.GetComponent<TextMeshProUGUI>();
        text.text = "";
    }

    public override void Interaction()
    {
        if (!isTalking) 
        {
            isTalking = true;
            base.Interaction();

            GameObject.Find("CM TargetGroup").GetComponent<CinemachineTargetGroup>().m_Targets[2].target = temp;
            GameObject.Find("CM TargetGroup").GetComponent<CinemachineTargetGroup>().m_Targets[2].weight = 5;

            StopAllCoroutines();
            StartCoroutine(OnType(time, comment));
        }
    }

    private IEnumerator OnType(float interval, string Say)
    {
        text.text = "";

        // F로 상호작용을 시작하고 차례로 여기까지 왔기 때문에 (F 누른 프레임과 같은 프레임에 실행되기 대문에) 바로 F 인풋을 검사하면 True로 나옴
        // 따라서 한 프레임 쉬어주고 검사 시작
        yield return null;
        StartCoroutine(CheckSkipInput());
        
        foreach (char item in Say)
        {
            if (!inputSkip)
            {
                text.text += item;
                yield return new WaitForSeconds(interval);
            }
            else
            {
                text.text = Say;
                break;
            }
        }

        StopCoroutine(CheckSkipInput());
        inputSkip = false;

        // 마찬가지로 만약 위에서 스킵한다면 F누른 직후 그대로 내려오기 때문에 가끔 밑에서 검사 할 때 바로 True로 나옴
        // 한 프레임 정도는 큰 부담 없으니까 기다리고 검사 시작
        yield return null;
        yield return StartCoroutine(WaitForTheInput());

        text.text = "";

        yield return null;
        StartCoroutine(CheckSkipInput());
        
        foreach (char item in comment2)
        {
            if (!inputSkip)
            {
                text.text += item;
                yield return new WaitForSeconds(interval);
            }
            else
            {
                text.text = comment2;
                break;
            }
        }

        StopCoroutine(CheckSkipInput());
        inputSkip = false;
    }

    private IEnumerator WaitForTheInput()
    {
        while (!Input.GetKeyDown(KeyCode.F))
        {
            yield return null;
        }
    }

    private IEnumerator CheckSkipInput()
    {
        // 체크하든 말든 직접 스탑 시켜줘야함 
        while (!Input.GetKeyDown(KeyCode.F))
        {
            yield return null;
        }
        inputSkip = true;
    }

    public void Rescue()
    {
        DataManager.Instance.SaveGameData(new GameData(true));
        GetComponent<SpriteRenderer>().enabled = false;
        Debug.Log("Sved");
        this.enabled = false;
    }

    protected override void OnTriggerExit2D(Collider2D collision)
    {
        base.OnTriggerExit2D(collision);
        isTalking = false;
        GameObject.Find("CM TargetGroup").GetComponent<CinemachineTargetGroup>().m_Targets[2].target = null;
        GameObject.Find("CM TargetGroup").GetComponent<CinemachineTargetGroup>().m_Targets[2].weight = 1;
    }
}
