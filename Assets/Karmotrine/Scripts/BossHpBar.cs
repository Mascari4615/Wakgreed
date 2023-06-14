using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BossHpBar : MonoBehaviour
{
    [SerializeField] private Image bossSprite;
    [SerializeField] private Image red;
    [SerializeField] private Image yellow;
    [SerializeField] private TextMeshProUGUI bossName;

    private BossMonster boss;
    private bool isShowingHpBar;

    private void Update()
    {
        if (isShowingHpBar)
        {
            if (boss != null)
            {
                red.fillAmount = Mathf.Lerp(red.fillAmount, (float)boss.hp / boss.MaxHp, Time.deltaTime * 15f);
                yellow.fillAmount = Mathf.Lerp(yellow.fillAmount, red.fillAmount, Time.deltaTime * 2f);
            }
        }
    }

    public void HpBarOn(BossMonster boss)
    {
        this.boss = boss;
        bossSprite.sprite = boss.GetComponent<SpriteRenderer>().sprite;
        bossSprite.transform.parent.gameObject.SetActive(true);
        red.transform.parent.gameObject.SetActive(true);
        bossName.text = boss.mobName;
        bossName.gameObject.SetActive(true);
        isShowingHpBar = true;
    }

    public void HpBarOff()
    {
        isShowingHpBar = false;
        boss = null;
        bossSprite.transform.parent.gameObject.SetActive(false);
        red.transform.parent.gameObject.SetActive(false);
        bossName.gameObject.SetActive(false);
    }
}