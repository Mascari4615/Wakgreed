using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossHpBar : MonoBehaviour
{
    [SerializeField] private Image bossSprite;
    [SerializeField] private Image bossHpBarRed;
    [SerializeField] private TextMeshProUGUI bossHpText;

    private BossMonster boss;

    public void HpBarOn(BossMonster boss)
    {
        this.boss = boss;
        bossSprite.sprite = boss.GetComponent<SpriteRenderer>().sprite;
        bossSprite.transform.parent.gameObject.SetActive(true);
        bossHpBarRed.transform.parent.gameObject.SetActive(true);

        StartCoroutine(UpdateHpBar());
    }

    public void HpBarOff()
    {
        bossSprite.transform.parent.gameObject.SetActive(false);
        bossHpBarRed.transform.parent.gameObject.SetActive(false);
        
        StopAllCoroutines();
    }

    private IEnumerator UpdateHpBar()
    {
        while (true)
        {
            bossHpBarRed.fillAmount = Mathf.Lerp(bossHpBarRed.fillAmount, (float)boss.Hp / boss.MAXHp, Time.deltaTime * 30f);
            bossHpText.SetText($"{boss.Hp}<size=25>/{boss.MAXHp}");

            yield return null;
        }
    }
}
