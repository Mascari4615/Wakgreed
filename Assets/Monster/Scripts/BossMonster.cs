using System.Collections;
using UnityEngine;

public abstract class BossMonster : Monster
{
    protected override void OnEnable()
    {
        base.OnEnable();
        StartCoroutine(Ready());
    }

    private IEnumerator Ready()
    {
        UIManager.Instance.bossHpBar.HpBarOn(this);
        yield return StartCoroutine(UIManager.Instance.SpeedWagon_Boss(gameObject));
        yield return new WaitForSeconds(2f);
        StartCoroutine("Attack");
    }

    protected abstract IEnumerator Attack();

    private void OnDisable()
    {
        UIManager.Instance.bossHpBar.HpBarOff();
    }
}
