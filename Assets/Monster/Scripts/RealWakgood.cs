using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RealWakgood : BossMonster
{
    protected override IEnumerator Attack()
    {
        throw new System.NotImplementedException();
    }
    
    protected override IEnumerator Collapse()
    {        
        UIManager.Instance.bossHpBar.HpBarOff();
        (StageManager.Instance.CurrentRoom as BossRoom)?.RoomClear();
        GameManager.Instance.StartCoroutine(GameManager.Instance.Ending());
        yield return base.Collapse();
    }
}
