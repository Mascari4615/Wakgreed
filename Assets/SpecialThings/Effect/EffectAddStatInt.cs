using UnityEngine;

[CreateAssetMenu(fileName = "EffectAddStatInt", menuName = "Effect/AddStatInt")]
public class EffectAddStatInt : Effect
{
    public IntVariable stat;
    public int value;

    public override void _Effect()
    {
        stat.RuntimeValue += value;
    }

    public override void Return()
    {
        stat.RuntimeValue -= value;
    }
}
