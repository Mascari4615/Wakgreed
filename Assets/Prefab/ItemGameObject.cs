using UnityEngine;

public class ItemGameObject : LootGameObject
{
    [SerializeField] private ItemDataBuffer ItemDataBuffer;
    [SerializeField] private ItemInventory ItemInventory;
    private Item item;

    public void SetItemGameObject(int id)
    {
        item = ItemDataBuffer.Items[id];
        spriteRenderer.sprite = item.sprite;
    }

    protected override void OnEquip()
    {
        ItemInventory.Add(item);
    }
}
