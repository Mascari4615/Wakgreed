using UnityEngine;

[CreateAssetMenu(fileName = "ItemEffectAddStatFloat", menuName = "ItemEffect/AddStatFloat" , order = 0)]
public class ItemEffectAddStatFloat : ItemEffect
{
    public FloatVariable stat;
    public float value;

    public override void Effect()
    {
        stat.RuntimeValue += value;
    }
}
