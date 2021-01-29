using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2 : Traveller
{
    public override void Update()
    {
        weaponPos.rotation = attackPosParent.transform.rotation;
        base.Update();
    }

    public override void Attack()
    {
        ObjectManager.Instance.GetQueue(PoolType.PlayerDefaultAttack2, attackPos);
    }
}
