public class ItemGameObject : LootGameObject
{
    private Item item;

    public void Initialize(int id)
    {
        item = DataManager.Instance.ItemDic[id];
        spriteRenderer.sprite = item.sprite;
    }

    protected override void OnEquip()
    {
        DataManager.Instance.WakgoodItemInventory.Add(item);
    }
}
