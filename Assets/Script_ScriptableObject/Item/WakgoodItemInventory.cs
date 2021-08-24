using UnityEngine;

[CreateAssetMenu(fileName = "WakgoodItemInventory", menuName = "GameSystem/RunTimeSet/WakgoodItemInventory")]
public class WakgoodItemInventory : ItemInventory
{
    [SerializeField] private GameEvent OnItemEquip;
    [SerializeField] private GameEvent OnItemRemove;

    public override void Add(Item item)
    {
        base.Add(item);
        item.OnEquip();
        OnItemEquip.Raise();
    }

    public override void Remove(Item item)
    {
        base.Remove(item);
        item.OnRemove();
        OnItemRemove.Raise();
    }
}
