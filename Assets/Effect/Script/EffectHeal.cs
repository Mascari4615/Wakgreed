using UnityEngine;

[CreateAssetMenu(fileName = "EffectHeal", menuName = "Effect/Heal")]
public class EffectHeal : Effect
{
    public IntVariable HP;
    public IntVariable MaxHP;
    public GameEvent OnHPChange;

    public override void _Effect()
    {
        HP.RuntimeValue = MaxHP.RuntimeValue;
        OnHPChange.Raise();
    }

    public override void Return()
    {
        
    }
}
