using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ItemEffectAddStat : ItemEffect
{
    public TravellerStat stat;
    public float value;

    public override void Effect(Traveller t)
    {
        t.AddStat(stat, value);
    }
}
