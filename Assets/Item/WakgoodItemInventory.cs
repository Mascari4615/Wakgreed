using UnityEngine;


[CreateAssetMenu(fileName = "WakgoodItemInventory", menuName = "GameSystem/RunTimeSet/WakgoodItemInventory")]
public class WakgoodItemInventory : ItemInventory
{
    [SerializeField] private ItemVariable LastEquippedItem;
    [SerializeField] private GameEvent OnItemEquip;
    [SerializeField] private GameEvent OnItemRemove;

    public override void Add(Item item)
    {
        base.Add(item);
        item.OnEquip();
        LastEquippedItem.RuntimeValue = item;
        OnItemEquip.Raise();
    }

    public override void Remove(Item item)
    {
        Debug.Log($"Remove : {item.name}");
        item.OnRemove();

        base.Remove(item);
        OnItemRemove.Raise();
    }
}
