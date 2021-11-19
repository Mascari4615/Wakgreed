using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossHpBar : MonoBehaviour
{
    [SerializeField] private Image bossSprite;
    [SerializeField] private Image red;
    [SerializeField] private Image yellow;
    [SerializeField] private TextMeshProUGUI bossHpText;

    private BossMonster boss;

    public void HpBarOn(BossMonster boss)
    {
        this.boss = boss;
        bossSprite.sprite = boss.GetComponent<SpriteRenderer>().sprite;
        bossSprite.transform.parent.gameObject.SetActive(true);
        red.transform.parent.gameObject.SetActive(true);

        StartCoroutine(UpdateHpBar());
    }

    public void HpBarOff()
    {
        bossSprite.transform.parent.gameObject.SetActive(false);
        red.transform.parent.gameObject.SetActive(false);
        
        StopAllCoroutines();
    }

    private IEnumerator UpdateHpBar()
    {
        while (true)
        {
            red.fillAmount = Mathf.Lerp(red.fillAmount, (float)boss.Hp / boss.MAXHp, Time.deltaTime * 15f);
            yellow.fillAmount = Mathf.Lerp(yellow.fillAmount, red.fillAmount, Time.deltaTime * 5f);
  
            if (Mathf.Abs(red.fillAmount - yellow.fillAmount) < 0.002f)
                yellow.fillAmount = red.fillAmount;
            
            bossHpText.SetText($"{boss.Hp}<size=25>/{boss.MAXHp}");

            yield return null;
        }
    }
}
