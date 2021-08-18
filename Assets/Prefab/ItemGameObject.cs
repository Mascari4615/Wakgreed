using UnityEngine;

public class ItemGameObject : LootGameObject
{
    [SerializeField] private ItemDataBuffer ItemDataBuffer;
    [SerializeField] private ItemInventory ItemInventory;
    private Item item;

    public void SetItemGameObject(int id = -1)
    {
        if (id == -1) id = Random.Range(0, ItemDataBuffer.Items.Length);
        item = ItemDataBuffer.Items[id];
        spriteRenderer.sprite = item.sprite;
    }

    protected override void OnEquip()
    {
        ItemInventory.Add(item);
    }
}
