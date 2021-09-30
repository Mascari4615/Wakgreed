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
        // GameEventListener 클래스를 통해 꼼수로 코루틴 실행하기
        StartCoroutine(GetDonation());
        StartCoroutine(CheckViewer());
    }

    private IEnumerator GetDonation()
    {
        WaitForSeconds ws = new(5f);

        // GameEventListener 클래스를 통해 꼼수로 코루틴 종료하기 (StopCoroutime 실행)
        while (true)
        {
            Viewer.RuntimeValue -= Random.Range(1, 10 + 1);

            if (Random.Range(0, 100) < 30)
            {
                int donationAmount = Random.Range(1, 1001);
                Nyang.RuntimeValue += donationAmount;

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
            Viewer.RuntimeValue -= Random.Range(1, 3 + 1);
            yield return ws;
        }
    }

    public void GetViewer(int amount)
    {
        Viewer.RuntimeValue += amount;
    }
}