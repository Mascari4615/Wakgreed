using UnityEngine;

[CreateAssetMenu(fileName = "EffectSetBoolVariable", menuName = "Effect/SetBoolVariable")]
public class EffectSetBoolVariable : Effect
{
    [SerializeField] private BoolVariable boolVariable;

    public override void Return()
    {
        boolVariable.RuntimeValue = false;
    }

    public override void _Effect()
    {
        boolVariable.RuntimeValue = true;
    }
}