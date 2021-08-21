using UnityEngine;

public class ItemGameObject : LootGameObject
{
    [SerializeField] private ItemInventory ItemInventory;
    private Item item;

    public void Initialize(int id)
    {
        item = DataManager.Instance.ItemDic[id];
        spriteRenderer.sprite = item.sprite;
    }

    protected override void OnEquip()
    {
        ItemInventory.Add(item);
    }
}
