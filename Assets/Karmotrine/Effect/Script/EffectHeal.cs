using UnityEngine;

[CreateAssetMenu(fileName = "EffectHeal", menuName = "Effect/Heal")]
public class EffectHeal : Effect
{
    public int Amount;

    public override void _Effect()
    {
        Wakgood.Instance.ReceiveHeal(Amount);
    }

    public override void Return()
    {
    }
}