using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Archer : TravellerFunctions
{
    public override void Initialize(Traveller t)
    {
        
    }

    public override void _Update(Traveller t)
    {
        t.weaponPosition.rotation = t.attackPositionParent.transform.rotation;    
    }

    public override void BasicAttack(Traveller t)
    {
        ObjectManager.Instance.GetQueue(PoolType.ArcherArrow, t.attackPosition);
    }

    public override void Ability0(Traveller t)
    {
        
    }

    public override void Ability1(Traveller t)
    {
        
    }

    public override void Ability2(Traveller t)
    {
        
    }
}
