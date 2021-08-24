using UnityEngine;

[CreateAssetMenu(fileName = "EffectAddStatFloat", menuName = "Effect/AddStatFloat")]
public class EffectAddStatFloat : Effect
{
    public GameEvent GameEvent;
    public FloatVariable stat;
    public float value;

    public override void _Effect()
    {
        stat.RuntimeValue += value;
        if(GameEvent!=null) GameEvent.Raise();
    }

    public override void Return()
    {
        stat.RuntimeValue -= value;
        if (GameEvent != null) GameEvent.Raise();

    }
}
