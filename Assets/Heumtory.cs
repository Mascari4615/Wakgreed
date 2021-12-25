using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FMODUnity;
using TMPro;

public class Heumtory : MonoBehaviour
{
    [TextArea][SerializeField] private List<string> texts;
    [SerializeField] private TextMeshProUGUI chatText;
    [SerializeField] private List<Sprite> sprites;
    [SerializeField] private Image image;
    private readonly WaitForSeconds ws5 = new(4f), ws005 = new(0.05f);

    private void OnEnable()
    {
        AudioManager.Instance.PlayMusic("Badassgatsby - °¡Áî¾Æ");
        StartCoroutine("Show");
    }

    private IEnumerator Show()
    {
        chatText.text = "";
        yield return new WaitForSeconds(1f);


        for (int i = 0; i < texts.Count; i++)
        {
            string text = texts[i];
            StartCoroutine(Imagee());
            chatText.text = "";

            foreach (char c in text)
            {
                chatText.text += c;
                RuntimeManager.PlayOneShot("event:/SFX/ETC/Text", transform.position);
                yield return ws005;
            }

            if (i == texts.Count - 1)
            {
                yield return new WaitForSeconds(1.8f);

            }
            else
            {
                yield return ws5;
            }
        }

        yield return new WaitForSeconds(1f);
        DataManager.Instance.CurGameData.youtubeHi = false;
        DataManager.Instance.SaveGameData();
        gameObject.SetActive(false);
    }

    private int stack = 0;
    private int[] ang = { 2, 5, 2, 1, 2, 3, 1};
    private IEnumerator Imagee()
    {
        float time = 4.5f;
        for (int i = 0; i < ang[stack]; i++)
        {
            image.sprite = sprites[0];
            sprites.RemoveAt(0);
            yield return new WaitForSeconds(time / ang[stack]);
        }
        stack++;
    }
}
