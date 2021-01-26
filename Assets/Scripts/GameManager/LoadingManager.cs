using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoBehaviour
{
    /*
    [SerializeField] private Slider progressBar;
    [SerializeField] private Text progressText;
    [SerializeField] private Text lodingComment;

    private void Start()
    {
        StartCoroutine(LoadScene());    
    }

    IEnumerator LoadScene()
    {
        yield return null;
        AsyncOperation operation = SceneManager.LoadSceneAsync("Game");
        operation.allowSceneActivation = false;
        lodingComment.text = "개발 버전입니다.";

        while (!operation.isDone)
        {
            yield return null;
            if (progressBar.value < 0.99f)
            {
                progressText.text = "세포 " + (int)(progressBar.value * 100) + "% 미분되는 중";
                progressBar.value = Mathf.Lerp(progressBar.value, operation.progress + 0.1f, 0.05f);
            }
            else if (progressBar.value >= 0.99f)
            {
                progressBar.value = 1;
                progressText.text = "세포 100% 완전 미분됨!";
                yield return new WaitForSeconds(1);
                progressText.text = "잃어버린 세포 찾는 중...";
                yield return new WaitForSeconds(0.25f);
                progressText.text = "다른세포 내보내는 중...";
                yield return new WaitForSeconds(0.25f);
                progressText.text = "세포 완전히 적분됨!";
                yield return new WaitForSeconds(1);
                operation.allowSceneActivation = true;
            }
        }
    }
    */
}
