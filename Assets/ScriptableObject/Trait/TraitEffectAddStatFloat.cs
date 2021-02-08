using UnityEngine;

[CreateAssetMenu(fileName = "TraitEffectAddStatFloat", menuName = "TraitEffect/AddStatFloat" , order = 1)]
public class TraitEffectAddStatFloat : TraitEffect
{
    public FloatVariable stat;
    public float value;

    public override void Effect()
    {
        stat.RuntimeValue += value;
    }
}
