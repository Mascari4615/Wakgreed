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
        DataManager.Instance.WakgoodItemInventory.Add(item);
    }
}
