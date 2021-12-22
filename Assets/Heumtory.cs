using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using TMPro;

public class Heumtory : MonoBehaviour
{
    [TextArea][SerializeField] private List<string> texts;
    [SerializeField] private TextMeshProUGUI chatText;
    private readonly WaitForSeconds ws5 = new(4.8f), ws005 = new(0.05f);

    private void OnEnable()
    {
        AudioManager.Instance.PlayMusic("Badassgatsby - °¡Áî¾Æ");
        StartCoroutine("Show");
    }

    private IEnumerator Show()
    {
        chatText.text = "";
        yield return new WaitForSeconds(1f);

        foreach (string text in texts)
        {
            chatText.text = "";

            foreach (char c in text)
            {
                chatText.text += c;
                RuntimeManager.PlayOneShot("event:/SFX/ETC/Text", transform.position);
                yield return ws005;
            }

            yield return ws5;
        }

        yield return new WaitForSeconds(1f);

        gameObject.SetActive(false);
    }
}
