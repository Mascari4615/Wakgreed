using UnityEngine;

public class ExpOrb : Loot
{
    [SerializeField] private IntVariable EXP;
    [SerializeField] private GameEvent OnEXPChange;
    protected override void OnEquip()
    {
        EXP.RuntimeValue += Random.Range(10, 16);
        OnEXPChange.Raise();
    }
}
