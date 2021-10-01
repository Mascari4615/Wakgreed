using UnityEngine;

public class IntValueLoot : LootGameObject
{
    [SerializeField] private IntVariable intVariable;
    [SerializeField] private int min;
    [SerializeField] private int max;
    protected override void OnEquip()
    {
        intVariable.RuntimeValue += Random.Range(min, max + 1);
    }
}