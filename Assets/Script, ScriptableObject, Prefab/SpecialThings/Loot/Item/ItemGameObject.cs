using UnityEngine;

public class ItemGameObject : LootGameObject
{
    [SerializeField] private TreasureDataBuffer TreasureDataBuffer;
    [SerializeField] private LootDataBuffer LootDataBuffer;
    [SerializeField] private ItemInventory ItemInventory;
    private Item item;

    public void SetItemGameObject(int id)
    {
        item = TreasureDataBuffer.Items[id];
        spriteRenderer.sprite = item.sprite;
    }

    protected override void OnEquip()
    {
        ItemInventory.Add(item);
    }
}
