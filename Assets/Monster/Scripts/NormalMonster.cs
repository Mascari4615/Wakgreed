using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public abstract class NormalMonster : Monster
{
    private GameObject hpBar, casting;
    private Image red, yellow, blue;

    protected override void Awake()
    {
        base.Awake();

        hpBar = transform.Find("Mob_Canvas").Find("HpBar").gameObject;
        casting = transform.Find("Mob_Canvas").Find("Casting").gameObject;
        red = hpBar.transform.Find("Red").GetComponent<Image>();
        yellow = hpBar.transform.Find("Yellow").GetComponent<Image>();
        blue = casting.transform.Find("Blue").GetComponent<Image>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        hpBar.SetActive(false);
        casting.SetActive(false);
        red.fillAmount = 1;
        yellow.fillAmount = 1;
        blue.fillAmount = 0;
    }

    protected override void _ReceiveHit()
    {
        if (!hpBar.activeSelf)
            hpBar.SetActive(true);
    }

    private void Update()
    {
        red.fillAmount = Mathf.Lerp(red.fillAmount, (float)hp / MaxHp, Time.deltaTime * 20f);
        yellow.fillAmount = Mathf.Lerp(yellow.fillAmount, red.fillAmount, Time.deltaTime * 5f);
    }

    protected IEnumerator Casting(float time)
    {
        float cur = 0;
        casting.SetActive(true);
        while (cur < time)
        {
            blue.fillAmount = cur / time;
            yield return null;
            cur += Time.deltaTime;
        }
        casting.SetActive(false);
    }

    protected override IEnumerator Collapse()
    {
        hpBar.SetActive(false);
        casting.SetActive(false);

        yield return base.Collapse();
    }
}