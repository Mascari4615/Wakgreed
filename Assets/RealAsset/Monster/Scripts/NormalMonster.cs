using System.Collections;
using UnityEngine;

public abstract class NormalMonster : Monster
{
    private GameObject hpBarGameObject;
    private GameObject yellowParent;
    private SpriteRenderer yellow;
    private GameObject redParent;

    protected override void Awake()
    {
        base.Awake();
        hpBarGameObject = transform.Find("HpBar").gameObject;
        yellowParent = hpBarGameObject.transform.GetChild(2).gameObject;
        yellow = yellowParent.transform.GetChild(0).GetComponent<SpriteRenderer>();
        redParent = hpBarGameObject.transform.GetChild(3).gameObject;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        hpBarGameObject.SetActive(true);
        redParent.transform.localScale = Vector3.one;
        yellowParent.transform.localScale = Vector3.one;
    }

    private void Update()
    {
        redParent.transform.localScale = new Vector3(Mathf.Lerp(redParent.transform.localScale.x, (float)HP / maxHP, Time.deltaTime * 30f), 1, 1);

        yellow.color = new Color(1, 1, Mathf.Lerp(0, 1, Time.deltaTime * 30f));
        yellowParent.transform.localScale = new Vector3(Mathf.Lerp(yellowParent.transform.localScale.x, redParent.transform.localScale.x, Time.deltaTime * 15f), 1, 1);

        if (yellowParent.transform.localScale.x - 0.01f <= redParent.transform.localScale.x)
        {
            yellow.color = new Color(1, 1, 0);
            yellowParent.transform.localScale = new Vector3(redParent.transform.localScale.x, 1, 1);
        }
    }

    protected override IEnumerator Collapse()
    {
        hpBarGameObject.SetActive(false);
        yield return base.Collapse();      
    }
}
