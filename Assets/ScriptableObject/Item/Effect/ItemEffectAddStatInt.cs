using UnityEngine;

[CreateAssetMenu(fileName = "ItemEffectAddStatInt", menuName = "ItemEffect/AddStatInt" , order = 0)]
public class ItemEffectAddStatInt : ItemEffect
{
    public IntVariable stat;
    public int value;

    public override void Effect()
    {
        stat.RuntimeValue += value;
    }
}
