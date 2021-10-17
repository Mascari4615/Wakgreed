using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public abstract class NormalMonster : Monster
{
    private GameObject hpBar;
    private Image yellow;
    private Image red;

    protected override void Awake()
    {
        base.Awake();
        hpBar = transform.Find("Mob_Canvas").Find("HPBar").gameObject;
        yellow = hpBar.transform.GetChild(0).Find("Yellow").GetComponent<Image>();
        red = hpBar.transform.GetChild(0).Find("Red").GetComponent<Image>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        hpBar.SetActive(false);
        yellow.fillAmount = 1;
        red.fillAmount = 1;
    }

    protected override void _ReceiveHit()
    {
        if (hpBar.activeSelf == false) hpBar.SetActive(true);
    }

    private void Update()
    {
        red.fillAmount = Mathf.Lerp(red.fillAmount, (float)HP / maxHP, Time.deltaTime * 30f);
        yellow.fillAmount = Mathf.Lerp(yellow.fillAmount, red.fillAmount, Time.deltaTime * 10f);

        if (yellow.fillAmount - 0.05f <= red.fillAmount)
        {
            yellow.color = new Color(1, 1, 0);
            yellow.fillAmount = red.fillAmount;
        }
    }

    protected override IEnumerator Collapse()
    {
        hpBar.SetActive(false);
        yield return base.Collapse();
    }
}
