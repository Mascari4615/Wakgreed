using UnityEngine;

public class IntValueLoot : LootGameObject
{
    [SerializeField] private IntVariable intVariable;
    [SerializeField] private int min;
    [SerializeField] private int max;
    [SerializeField] private GameEvent gameEvent;
    protected override void OnEquip()
    {
        intVariable.RuntimeValue += Random.Range(min, max + 1);
        if (!(gameEvent is null)) gameEvent.Raise();
    }
}