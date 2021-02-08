using UnityEngine;

[CreateAssetMenu(fileName = "TraitEffectAddStatInt", menuName = "TraitEffect/AddStatInt" , order = 0)]
public class TraitEffectAddStatInt : TraitEffect
{
    public IntVariable stat;
    public int value;

    public override void Effect()
    {
        stat.RuntimeValue += value;
    }
}
