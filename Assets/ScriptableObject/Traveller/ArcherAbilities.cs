using UnityEngine;

[CreateAssetMenu(fileName = "ArcherAbilities", menuName = "TravellerAbilities/Archer")]
public class ArcherAbilities : TravellerAbilities
{
    public override void Initialize(TravellerController t)
    {
        
    }

    public override void _Update(TravellerController t)
    {
        t.weaponPosition.rotation = t.attackPositionParent.transform.rotation;    
    }

    public override void BasicAttack(TravellerController t)
    {
        //ObjectManager.Instance.GetQueue(PoolType.ArcherArrow, t.attackPosition);
        ObjectManager.Instance.GetQueue("Arrow", t.attackPosition);
    }

    public override void Skill0(TravellerController t)
    {
        
    }

    public override void Skill1(TravellerController t)
    {
        
    }

    public override void Skill2(TravellerController t)
    {
        
    }
}
