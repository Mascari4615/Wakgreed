using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemInventory", menuName = "GameSystem/RunTimeSet/ItemInventory")]
public class ItemInventory : RunTimeSet<Item>
{
    [SerializeField] private GameEvent OnItemEquip;
    [SerializeField] private GameEvent OnItemRemove;
    [System.NonSerialized] public Dictionary<int, int> itemCountDic = new();

    public override void Add(Item item)
    {
        if (!Items.Contains(item))
        {
            Items.Add(item);
            itemCountDic.Add(item.ID, 1);
        }
        else itemCountDic[item.ID] += 1;

        item.OnEquip();
        OnItemEquip.Raise();
    }

    public override void Remove(Item item)
    {
        if (Items.Contains(item))
        {
            if (itemCountDic[item.ID] > 1) itemCountDic[item.ID] -= 1;
            else if (itemCountDic[item.ID] == 1)
            {
                itemCountDic.Remove(item.ID);
                Items.Remove(item);
            }
            else Debug.LogError("RunTimeSet<Item> : Item.count는 음수가 될 수 없음");
        }
        else Debug.LogError("RunTimeSet<Item> : 존재하지 않는 아이템 제거 시도");

        item.OnRemove();
        OnItemRemove.Raise();
    }
}