using UnityEngine;

[CreateAssetMenu(fileName = "ArcherFunction", menuName = "TravellerFunctions/Archer" , order = 1)]
public class ArcherFunction : TravellerFunctions
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
        ObjectManager.Instance.GetQueue(PoolType.ArcherArrow, t.attackPosition);
    }

    public override void Ability0(TravellerController t)
    {
        
    }

    public override void Ability1(TravellerController t)
    {
        
    }

    public override void Ability2(TravellerController t)
    {
        
    }
}
