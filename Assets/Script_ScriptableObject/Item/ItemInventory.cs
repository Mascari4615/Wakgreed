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
            itemCountDic.Add(item.ID, 1);
        }
        else itemCountDic[item.ID] += 1;
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
            else Debug.LogError("RunTimeSet<Item> : Item.count�� ������ �� �� ����");
        }
        else Debug.LogError("RunTimeSet<Item> : �������� �ʴ� ������ ���� �õ�");
    }

    public override void Clear()
    {
        base.Clear();
        itemCountDic.Clear();
    }
}