using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public abstract class NormalMonster : Monster
{
    [SerializeField] protected float castingTime = .5f;
    private GameObject hpBar, casting;
    private Image red, yellow, blue;
    protected Vector3 spawnPos;

    protected override void Awake()
    {
        base.Awake();

        hpBar = transform.Find("Mob_Canvas").Find("HpBar").gameObject;
        casting = transform.Find("Mob_Canvas").Find("Casting").gameObject;
        red = hpBar.transform.Find("Red").GetComponent<Image>();
        yellow = hpBar.transform.Find("Yellow").GetComponent<Image>();
        blue = casting.transform.Find("Blue").GetComponent<Image>();
    }

    private void Update()
    {
        red.fillAmount = Mathf.Lerp(red.fillAmount, (float)hp / MaxHp, Time.deltaTime * 25f);
        yellow.fillAmount = Mathf.Lerp(yellow.fillAmount, red.fillAmount, Time.deltaTime * 2f);

        if (Vector3.Distance(transform.position, spawnPos) > 50)
        {
            Debug.LogWarning($"비정상적 움직임 : {gameObject.name}이 스폰 위치로부터 50 이상 떨어졌습니다.");
            onMonsterCollapse.Raise();
            gameObject.SetActive(false);
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        hpBar.SetActive(false);
        casting.SetActive(false);
        red.fillAmount = 1;
        yellow.fillAmount = 1;
        blue.fillAmount = 0;

        spawnPos = transform.position;
    }

    protected override void _ReceiveHit()
    {
        if (!hpBar.activeSelf)
        {
            hpBar.SetActive(true);
        }
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

    protected override void Collapse()
    {
        hpBar.SetActive(false);
        casting.SetActive(false);

        base.Collapse();
    }
}