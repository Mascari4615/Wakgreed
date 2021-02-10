using UnityEngine;

public abstract class TravellerAbilities : ScriptableObject
{
    public abstract void Initialize(TravellerController t);
    public abstract void _Update(TravellerController t);
    public abstract void BasicAttack(TravellerController t);
    public abstract void Skill0(TravellerController t);
    public abstract void Skill1(TravellerController t);
    public abstract void Skill2(TravellerController t);
}
