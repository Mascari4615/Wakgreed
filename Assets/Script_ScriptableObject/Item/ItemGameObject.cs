using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemGameObject : LootGameObject
{
    private Item item;

    public void Initialize(int id)
    {
        item = DataManager.Instance.ItemDic[id];
        spriteRenderer.sprite = item.sprite;
    }

    public void Initialize(ItemGrade itemGrade)
    {
        IEnumerable<int> itemRange = Enumerable.Range(0, 100);

        if (itemGrade == ItemGrade.Common) itemRange = Enumerable.Range(0, 100);
        else if (itemGrade == ItemGrade.Uncommon) itemRange = Enumerable.Range(100, 200);
        else if (itemGrade == ItemGrade.Legendary) itemRange = Enumerable.Range(200, 300);

        var itemList = (from itemID in DataManager.Instance.ItemDic.Keys where itemRange.Contains(itemID) select itemID).ToList();

        itemID = itemList[Random.Range(0, itemList.Count)];

        item = DataManager.Instance.ItemDic[itemID];
        spriteRenderer.sprite = item.sprite;
    }

    public void InitializeRandom()
    {
        ItemGrade itemGrade = (ItemGrade)Random.Range(0, 4);
        IEnumerable<int> itemRange = Enumerable.Range(0, 100);

        if (itemGrade == ItemGrade.Common) itemRange = Enumerable.Range(0, 100);
        else if (itemGrade == ItemGrade.Uncommon) itemRange = Enumerable.Range(100, 200);
        else if (itemGrade == ItemGrade.Legendary) itemRange = Enumerable.Range(200, 300);

        var itemList = (from itemID in DataManager.Instance.ItemDic.Keys where itemRange.Contains(itemID) select itemID).ToList();

        itemID = itemList[Random.Range(0, itemList.Count)];

        item = DataManager.Instance.ItemDic[itemID];
        spriteRenderer.sprite = item.sprite;
    }

    private int itemID;

    protected override void OnEquip()
    {
        DataManager.Instance.WakgoodItemInventory.Add(item);
    }
}
