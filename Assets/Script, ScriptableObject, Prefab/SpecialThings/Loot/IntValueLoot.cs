using UnityEngine;

public class IntValueLoot : LootGameObject
{
    private enum IntValueLootType
    {
        Nyang, Exp
    }
    [SerializeField] private IntValueLootType type;
    [SerializeField] private IntVariable intVariable;
    [SerializeField] private int min;
    [SerializeField] private int max;

    protected override void OnEquip()
    {
        intVariable.RuntimeValue += Random.Range(min, max + 1);
        if (type == IntValueLootType.Nyang) SoundManager.Instance.Nyang();
    }
}