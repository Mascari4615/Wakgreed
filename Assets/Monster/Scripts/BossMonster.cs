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
        StartCoroutine(nameof(Attack));
    }

    protected abstract IEnumerator Attack();
    
    protected override IEnumerator Collapse()
    {        
        UIManager.Instance.bossHpBar.HpBarOff();
        (StageManager.Instance.CurrentRoom as BossRoom)?.RoomClear();
        yield return base.Collapse();
    }
}
