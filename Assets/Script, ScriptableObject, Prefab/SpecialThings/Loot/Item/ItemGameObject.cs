using UnityEngine;

public class ItemGameObject : Loot
{
    [SerializeField] private ItemDataBuffer ItemDataBuffer;
    [SerializeField] private ItemInventory ItemInventory;
    private Item item;

    protected override void _OnEnable()
    {
        item = ItemDataBuffer.Items[Random.Range(0, ItemDataBuffer.Items.Length)];
        spriteRenderer.sprite = item.sprite;
    }

    protected override void OnEquip()
    {
        if (ItemInventory.Items.Contains(item))
            ItemInventory.Items[ItemInventory.Items.IndexOf(item)].count++;
        else ItemInventory.Add(item);

        item.OnEquip();
        gameObject.SetActive(false);
    }
}
