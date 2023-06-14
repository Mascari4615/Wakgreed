using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Synergy", menuName = "Variable/Synergy")]
public class Synergy : SpecialThing
{
    public Item[] SynergyItems;
    public Weapon SynergyWeapon;
    [NonSerialized] public bool isOn;
}