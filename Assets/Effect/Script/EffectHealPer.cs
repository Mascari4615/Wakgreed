using System;
using UnityEngine;

[CreateAssetMenu(fileName = "EffectHealPer", menuName = "Effect/HealPer")]

public class EffectHealPer : Effect
{
    public int Per;
    public MaxHp MaxHP;

    public override void _Effect()
    {
        Wakgood.Instance.ReceiveHeal((int)Math.Round(Per * (float)MaxHP.RuntimeValue / 100, MidpointRounding.AwayFromZero));
    }

    public override void Return()
    {

    }
}
