public class ItemGameObject : LootGameObject
{
    private Item item;

    public void Initialize(int itemID)
    {
        item = DataManager.Instance.ItemDic[itemID];
        spriteRenderer.sprite = item.sprite;
    }

    protected override void OnEquip()
    {
        if (DataManager.Instance.CurGameData.equipedOnceItem[item.id] == false)
        {
            if (Collection.Instance != null)
                Collection.Instance.Collect(item);
            DataManager.Instance.CurGameData.equipedOnceItem[item.id] = true;
            DataManager.Instance.SaveGameData();
        }

        DataManager.Instance.wakgoodItemInventory.Add(item);
    }
}
