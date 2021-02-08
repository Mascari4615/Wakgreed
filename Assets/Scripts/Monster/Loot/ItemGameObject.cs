using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGameObject : Loot
{
    [SerializeField] private ItemDataBuffer itemDataBuffer;
    [SerializeField] private Inventory inventory;
    private Item item;

    public override void OnEnable()
    {
        base.OnEnable();

        item = itemDataBuffer.Items[Random.Range(0, itemDataBuffer.Items.Length)];
        waitTime = 1f;
    }

    public override void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (inventory.Items.Contains(item))
            {
                inventory.Items[inventory.Items.IndexOf(item)].count++;
            }
            else
            {
                inventory.Items.Add(item);
            }

            item.OnEquip();

            Debug.Log($"{name} : InsertQueue");
            ObjectManager.Instance.InsertQueue(PoolType.Item, gameObject);
        }       
    }
}
