using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player2 : Traveller
{
    protected override void Update()
    {
        base.Update();
        
        weaponPosition.rotation = attackPositionParent.transform.rotation;    
    }

    protected override void Attack()
    {
        base.Attack();

        ObjectManager.Instance.GetQueue(PoolType.PlayerDefaultAttack2, attackPosition);
    }
}
