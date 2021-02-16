using UnityEngine;

public class ItemGameObject : LootGameObject
{
    [SerializeField] private TreasureDataBuffer TreasureDataBuffer;
    [SerializeField] private LootDataBuffer LootDataBuffer;
    [SerializeField] private ItemInventory ItemInventory;
    private Item item;

    public void SetItemGameObject(int id, bool isTreasure = false)
    {
        if (isTreasure)
        {
            item = TreasureDataBuffer.Items[id];
        }
        else
        {
            item = LootDataBuffer.Items[id];
        }
        spriteRenderer.sprite = item.sprite;
    }

    protected override void OnEquip()
    {
        ItemInventory.Add(item);
    }
}
