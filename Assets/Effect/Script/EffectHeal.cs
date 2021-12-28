using UnityEngine;

[CreateAssetMenu(fileName = "EffectHeal", menuName = "Effect/Heal")]
public class EffectHeal : Effect
{
    public int Amount;
    public IntVariable HP;
    public MaxHp MaxHP;
    public GameEvent OnHPChange;

    public override void _Effect()
    {
        HP.RuntimeValue = Mathf.Clamp(HP.RuntimeValue + Amount, 0, MaxHP.RuntimeValue);
        ObjectManager.Instance.PopObject("AnimatedText", Wakgood.Instance.transform).GetComponent<AnimatedText>().SetText(Amount.ToString(), Color.green);
        OnHPChange.Raise();
    }

    public override void Return()
    {
        
    }
}
