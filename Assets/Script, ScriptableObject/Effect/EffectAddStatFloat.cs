using UnityEngine;

[CreateAssetMenu(fileName = "EffectAddStatFloat", menuName = "Effect/AddStatFloat")]
public class EffectAddStatFloat : Effect
{
    public FloatVariable stat;
    public float value;

    public override void _Effect()
    {
        stat.RuntimeValue += value;
    }

    public override void Return()
    {
        stat.RuntimeValue -= value;   
    }
}
