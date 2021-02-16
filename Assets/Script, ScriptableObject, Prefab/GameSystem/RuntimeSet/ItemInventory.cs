using UnityEngine;

[CreateAssetMenu(fileName = "ItemInventory", menuName = "GameSystem/RunTimeSet/ItemInventory")]
public class ItemInventory : RunTimeSet<Item>
{
    [SerializeField] private GameEvent OnItemInventoryChange;

    public override void Add(Item t)
    {
        base.Add(t);
        OnItemInventoryChange.Raise();
    }

    public override void Remove(Item t)
    {
        base.Remove(t);
        OnItemInventoryChange.Raise();
    }
}