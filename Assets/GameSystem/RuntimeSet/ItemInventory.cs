using UnityEngine;

[CreateAssetMenu(fileName = "ItemInventory", menuName = "GameSystem/RunTimeSet/ItemInventory")]
public class ItemInventory : RunTimeSet<Item>
{
    [SerializeField] private GameEvent OnItemEquip;
    [SerializeField] private GameEvent OnItemRemove;

    public override void Add(Item item)
    {
        // Debug.Log($"{name} : Add");
        base.Add(item);
        // Debug.Log($"{name} : Add 2 Raise");
        item.OnEquip();
        OnItemEquip.Raise();
    }

    public override void Remove(Item item)
    {
        // Debug.Log($"{name} : Remove");
        base.Remove(item);
        // Debug.Log($"{name} : Remove 2 Raise");
        item.OnRemove();
        OnItemRemove.Raise();
    }
}