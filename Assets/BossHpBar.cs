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
    [SerializeField] private TextMeshProUGUI bossName;

    private BossMonster boss = null;
    private bool asd = false;

    public void HpBarOn(BossMonster boss)
    {
        Debug.Log("On");
        this.boss = boss;
        bossSprite.sprite = boss.GetComponent<SpriteRenderer>().sprite;
        bossSprite.transform.parent.gameObject.SetActive(true);
        red.transform.parent.gameObject.SetActive(true);
        bossName.text = boss.mobName;
        bossName.gameObject.SetActive(true);
        asd = true;
    }

    public void HpBarOff()
    {
        Debug.Log("Off");
        asd = false;
        boss = null;
        bossSprite.transform.parent.gameObject.SetActive(false);
        red.transform.parent.gameObject.SetActive(false);
        bossName.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (asd)
        {
            Debug.Log("ING");

            if (boss != null)
            {
                red.fillAmount = Mathf.Lerp(red.fillAmount, (float)boss.hp / boss.MaxHp, Time.deltaTime * 15f);
                yellow.fillAmount = Mathf.Lerp(yellow.fillAmount, red.fillAmount, Time.deltaTime * 2f);
            }
        }
    }
}
