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

        // F�� ��ȣ�ۿ��� �����ϰ� ���ʷ� ������� �Ա� ������ (F ���� �����Ӱ� ���� �����ӿ� ����Ǳ� �빮��) �ٷ� F ��ǲ�� �˻��ϸ� True�� ����
        // ���� �� ������ �����ְ� �˻� ����
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

        // ���������� ���� ������ ��ŵ�Ѵٸ� F���� ���� �״�� �������� ������ ���� �ؿ��� �˻� �� �� �ٷ� True�� ����
        // �� ������ ������ ū �δ� �����ϱ� ��ٸ��� �˻� ����
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
        // üũ�ϵ� ���� ���� ��ž ��������� 
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
