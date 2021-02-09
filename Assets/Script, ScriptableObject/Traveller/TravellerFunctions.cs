using UnityEngine;

public abstract class TravellerFunctions : ScriptableObject
{
    public abstract void Initialize(TravellerController t);
    public abstract void _Update(TravellerController t);
    public abstract void BasicAttack(TravellerController t);
    public abstract void Ability0(TravellerController t);
    public abstract void Ability1(TravellerController t);
    public abstract void Ability2(TravellerController t);
}
