using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TravellerFunctions : ScriptableObject
{
    public abstract void Initialize(Traveller t);
    public abstract void _Update(Traveller t);
    public abstract void BasicAttack(Traveller t);
    public abstract void Ability0(Traveller t);
    public abstract void Ability1(Traveller t);
    public abstract void Ability2(Traveller t);
}
