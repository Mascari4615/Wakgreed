using UnityEngine;

[CreateAssetMenu(fileName = "EffectAddStatInt", menuName = "Effect/AddStatInt")]
public class EffectAddStatInt : Effect
{
    public GameEvent GameEvent;
    public IntVariable stat;
    public int value;

    public override void _Effect()
    {
        stat.RuntimeValue += value;
        if (GameEvent != null) GameEvent.Raise();
    }

    public override void Return()
    {
        stat.RuntimeValue -= value;
        if (GameEvent != null) GameEvent.Raise();
    }
}
