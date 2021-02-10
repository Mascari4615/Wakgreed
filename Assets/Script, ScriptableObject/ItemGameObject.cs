using UnityEngine;

public class ItemGameObject : Loot
{
    [SerializeField] private ItemDataBuffer ItemDataBuffer;
    [SerializeField] private Inventory Inventory;
    [SerializeField] private GameEvent OnEquipItem;
    private Item item;

    protected override void _OnEnable()
    {
        item = ItemDataBuffer.Items[Random.Range(0, ItemDataBuffer.Items.Length)];
        spriteRenderer.sprite = item.sprite;
        waitTime = 1f;
    }

    protected override void OnEquip()
    {
        if (Inventory.Items.Contains(item))
            Inventory.Items[Inventory.Items.IndexOf(item)].count++;
        else Inventory.Add(item);

        item.OnEquip();
        OnEquipItem.Raise();
    }
}
