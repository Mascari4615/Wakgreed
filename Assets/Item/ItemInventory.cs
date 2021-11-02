using System.Collections.Generic;
using UnityEngine;

public abstract class ItemInventory : RunTimeSet<Item>
{
    [System.NonSerialized] public Dictionary<int, int> itemCountDic = new();

    public override void Add(Item item)
    {
        if (!Items.Contains(item))
        {
            Items.Add(item);
            itemCountDic.Add(item.id, 1);
        }
        else itemCountDic[item.id] += 1;
    }

    public override void Remove(Item item)
    {
        if (Items.Contains(item))
        {
            if (itemCountDic[item.id] > 1) itemCountDic[item.id] -= 1;
            else if (itemCountDic[item.id] == 1)
            {
                itemCountDic.Remove(item.id);
                Items.Remove(item);
            }
            else Debug.LogError("RunTimeSet<Item> : Item.count는 음수가 될 수 없음");
        }
        else Debug.LogError("RunTimeSet<Item> : 존재하지 않는 아이템 제거 시도");
    }

    public override void Clear()
    {
        if (Items == null) return;

        int itemsCount = Items.Count;
        for (int i = 0; i < itemsCount; i++)
        {
            int itemCount = itemCountDic[Items[0].id];
            for (int j = 0; j < itemCount; j++)
            {
                Remove(Items[0]);
            }         
        }

        Items.Clear();
        itemCountDic.Clear();
    }
}