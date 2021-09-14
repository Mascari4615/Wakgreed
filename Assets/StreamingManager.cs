using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StreamingManager : MonoBehaviour
{
    [SerializeField] private GameObject donationUI;
    [SerializeField] private GameObject[] viewerGraphDot;
    [SerializeField] private GameObject[] viewerGraphLine;
    [SerializeField] private TextMeshProUGUI curViewerMaxText;
    [SerializeField] private TextMeshProUGUI curViewerMinText;
    [SerializeField] private IntVariable Nyang;
    [SerializeField] private IntVariable Viewer;
    private Queue<int> viewerQueue = new();
    int[] viewerHistory;

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
            }

            yield return ws;
        }
    }

    private IEnumerator CheckViewer()
    {
        WaitForSeconds ws = new(.5f);
        int curViewerMax = 5000;
        int curViewerMin = 2500;
        curViewerMinText.text = curViewerMin.ToString();
        curViewerMaxText.text = curViewerMax.ToString();
        viewerHistory = new int[viewerGraphDot.Length];

        StartCoroutine(UpdateViewerUI());

        // GameEventListener 클래스를 통해 꼼수로 코루틴 종료하기 (StopCoroutime 실행)
        while (true)
        {
            Viewer.RuntimeValue -= Random.Range(1, 3 + 1);
            if (Viewer.RuntimeValue <= curViewerMin + 200)
            {
                curViewerMin -= 200;
                curViewerMax -= 200;
                curViewerMinText.text = curViewerMin.ToString();
                curViewerMaxText.text = curViewerMax.ToString();

                for (int j = 0; j < viewerGraphDot.Length - 1; j++)
                {
                    viewerGraphDot[j].transform.localPosition = new Vector3(
                        viewerGraphDot[j].transform.localPosition.x,
                        (2 * ((viewerHistory[j] - curViewerMin) / (float)((curViewerMax - curViewerMin))) - 1) * 80,
                        0);
                }
            }

            int viewerQueueCount = viewerQueue.Count;
            for (int i = 0; i < viewerQueueCount; i++)
            {
                Viewer.RuntimeValue += viewerQueue.Dequeue();
                if (Viewer.RuntimeValue >= curViewerMax - 500)
                {
                    curViewerMin += 500;
                    curViewerMax += 500;
                    curViewerMinText.text = curViewerMin.ToString();
                    curViewerMaxText.text = curViewerMax.ToString();

                    for (int j = 0; j < viewerGraphDot.Length - 1; j++)
                    {
                        viewerGraphDot[j].transform.localPosition = new Vector3(
                            viewerGraphDot[j].transform.localPosition.x,
                            (2 * ((viewerHistory[j] - curViewerMin) / (float)((curViewerMax - curViewerMin))) - 1) * 80,
                            0);
                    }   
                }
            }

            viewerHistory[4] = Viewer.RuntimeValue;
            viewerGraphDot[4].transform.localPosition = new Vector3(
                viewerGraphDot[4].transform.localPosition.x,
                (2 * ((Viewer.RuntimeValue - curViewerMin) / (float)((curViewerMax - curViewerMin))) - 1) * 80,
                0);

            yield return ws;
        }
    }

    private IEnumerator UpdateViewerUI()
    {
        WaitForSeconds ws = new(20f);

        while (true)
        {
            for (int i = 0; i < viewerGraphDot.Length - 1; i++)
            {
                viewerHistory[i] = viewerHistory[i + 1];
                viewerGraphDot[i].transform.localPosition = new Vector3(
                viewerGraphDot[i].transform.localPosition.x,
                viewerGraphDot[i + 1].transform.localPosition.y,
                0);
            }

            yield return ws;
        }
    }

    public void GetViewer(int amount)
    {
        viewerQueue.Enqueue(amount);
    }
}