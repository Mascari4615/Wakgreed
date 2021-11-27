using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public abstract class NormalMonster : Monster
{
    private GameObject hpBar;
    private Image yellow;
    private Image red;
    private GameObject casting;
    private Image blue;
    
    protected override void Awake()
    {
        base.Awake();
        hpBar = transform.Find("Mob_Canvas").Find("HpBar").gameObject;
        yellow = hpBar.transform.Find("Yellow").GetComponent<Image>();
        red = hpBar.transform.Find("Red").GetComponent<Image>();
        casting = transform.Find("Mob_Canvas").Find("Casting").gameObject;
        blue = casting.transform.Find("Blue").GetComponent<Image>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        hpBar.SetActive(false);
        casting.SetActive(false);
        yellow.fillAmount = 1;
        red.fillAmount = 1;
        blue.fillAmount = 0;
    }

    protected override void _ReceiveHit()
    {
        if (hpBar.activeSelf == false)
            hpBar.SetActive(true);
    }

    private void Update()
    {
        red.fillAmount = Mathf.Lerp(red.fillAmount, (float)Hp / MAXHp, Time.deltaTime * 30f);
        yellow.fillAmount = Mathf.Lerp(yellow.fillAmount, red.fillAmount, Time.deltaTime * 10f);

        if (!(yellow.fillAmount - 0.05f <= red.fillAmount))
        {
            return;
        }
        
        yellow.fillAmount = red.fillAmount;
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
        yield return base.Collapse();
    }
}
