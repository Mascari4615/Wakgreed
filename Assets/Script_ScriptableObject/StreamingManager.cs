using FMODUnity;
using System.Collections;
using TMPro;
using UnityEngine;

public class StreamingManager : MonoBehaviour
{
    [SerializeField] private GameObject donationUI;
    [SerializeField] private IntVariable Nyang;
    [SerializeField] private IntVariable Viewer;

    public void StartStreaming()
    {
        // GameEventListener Ŭ������ ���� �ļ��� �ڷ�ƾ �����ϱ�
        StartCoroutine(GetDonation());
        StartCoroutine(CheckViewer());
    }

    private IEnumerator GetDonation()
    {
        WaitForSeconds ws = new(5f);

        // GameEventListener Ŭ������ ���� �ļ��� �ڷ�ƾ �����ϱ� (StopCoroutime ����)
        while (true)
        {
            Viewer.RuntimeValue -= Random.Range(1, 10 + 1);

            if (Random.Range(0, 100) < 30)
            {
                int donationAmount = Random.Range(1, 1001);
                Nyang.RuntimeValue += donationAmount;

                // �ļ��� �ִϸ��̼� �����ϱ�
                // # UI�� �и���Ű�� �۾� �ʿ�
                donationUI.SetActive(false);
                donationUI.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = $"Ttmdacl�� ���� {donationAmount}��� ����";
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
            Viewer.RuntimeValue -= Random.Range(1, 3 + 1);
            yield return ws;
        }
    }

    public void GetViewer(int amount)
    {
        Viewer.RuntimeValue += amount;
    }
}