using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Item : ScriptableObject
{
    public int ID;
    public string iName;
    public string description;
    public Sprite sprite ;
    public ItemEffect[] effects;

    public void OnEquip(Traveller t)
    {
        for (int i = 0; i < effects.Length; i++)
        {
            effects[i].Effect(t);
        }
    }
}
