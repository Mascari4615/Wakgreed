using UnityEngine;

[CreateAssetMenu(fileName = "EffectAddMaxHp", menuName = "Effect/AddMaxHp")]
public class EffectAddMaxHp : Effect
{
    public MaxHp stat;
    public int value;
    public override void _Effect() => stat.RuntimeValue += value;
    public override void Return() => stat.RuntimeValue -= value;
}
