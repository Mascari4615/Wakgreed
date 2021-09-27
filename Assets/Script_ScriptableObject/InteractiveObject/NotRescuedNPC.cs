using Cinemachine;
using FMODUnity;
using System.Collections;
using TMPro;
using UnityEngine;

public class NotRescuedNPC : NPC
{
    private TextMeshProUGUI text;

    [SerializeField] private float time;

    [TextArea] [SerializeField] private string comment;
    [TextArea] [SerializeField] private string comment2;

    [SerializeField] private Transform temp;

    private bool isTalking = false;
    private bool inputSkip = false;

    private void Start()
    {
        text = ui.GetComponent<TextMeshProUGUI>();
        text.text = "";
    }

    public override void Interaction()
    {
        if (!isTalking)
        {
            isTalking = true;
            inputSkip = false;
            base.Interaction();

            GameObject.Find("CM TargetGroup").GetComponent<CinemachineTargetGroup>().m_Targets[2].target = temp;
            GameObject.Find("CM TargetGroup").GetComponent<CinemachineTargetGroup>().m_Targets[2].weight = 5;

            StopAllCoroutines();
            StartCoroutine(OnType(time, comment));
        }
    }

    private IEnumerator OnType(float interval, string Say)
    {
        IEnumerator CheckSkip;

    ASD:
        text.text = "";

        // F�� ��ȣ�ۿ��� �����ϰ� ���ʷ� ������� �Ա� ������ (F ���� �����Ӱ� ���� �����ӿ� ����Ǳ� �빮��) �ٷ� F ��ǲ�� �˻��ϸ� True�� ����
        // ���� �� ������ �����ְ� �˻� ����
        CheckSkip = CheckSkipInput();
        StartCoroutine(CheckSkip);
        foreach (char item in Say)
        {
            //Debug.Log($"Ȧ���� ���ϴ��� : {item}");
            if (!inputSkip)
            {
                text.text += item;
                // RuntimeManager.PlayOneShot("event:/New Event", transform.position);
                yield return new WaitForSeconds(interval);
            }
            else
            {
                text.text = Say;
                break;
            }
        }
        StopCoroutine(CheckSkip);
        inputSkip = false;

        // ���������� ���� ������ ��ŵ�Ѵٸ� F���� ���� �״�� �������� ������ ���� �ؿ��� �˻� �� �� �ٷ� True�� ����
        // �� ������ ������ ū �δ� �����ϱ� ��ٸ��� �˻� ����

        yield return new WaitForSeconds(0.2f);
        yield return StartCoroutine(WaitForTheInput());

        text.text = "";

        CheckSkip = CheckSkipInput();
        StartCoroutine(CheckSkip);
        foreach (char item in comment2)
        {
            // Debug.Log($"��Űŷ ���ϴ��� : {item}");
            if (!inputSkip)
            {
                text.text += item;
                // RuntimeManager.PlayOneShot("event:/New Event", transform.position);
                yield return new WaitForSeconds(interval);
            }
            else
            {
                text.text = comment2;
                break;
            }
        }
        StopCoroutine(CheckSkip);
        inputSkip = false;

        yield return new WaitForSeconds(0.2f);
        yield return StartCoroutine(WaitForTheInput());

        goto ASD;
    }

    private IEnumerator WaitForTheInput()
    {
        yield return null;
        while (!Input.GetKeyDown(KeyCode.F))
        {
            //Debug.Log("���� ��ǲ �����");
            yield return null;
        }
        //Debug.Log("���� ��ǲ Ż��");
    }

    private IEnumerator CheckSkipInput()
    {
        yield return null;
        while (!Input.GetKeyDown(KeyCode.F))
        {
            //Debug.Log("��ŵ ��ǲ �����");
            yield return null;
        }
        inputSkip = true;
        //Debug.Log("��ŵ ��ǲ Ż��");
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
        StopAllCoroutines();
        isTalking = false;
        inputSkip = false;
        GameObject.Find("CM TargetGroup").GetComponent<CinemachineTargetGroup>().m_Targets[2].target = null;
        GameObject.Find("CM TargetGroup").GetComponent<CinemachineTargetGroup>().m_Targets[2].weight = 1;
    }
}
