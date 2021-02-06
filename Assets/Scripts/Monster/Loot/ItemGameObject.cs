using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGameObject : Loot
{
    [HideInInspector] public int itemID;
    public void SetItem(Item item)
    {
        itemID = item.itemID;
        this.name = item.itemName;
        GetComponent<SpriteRenderer>().sprite = item.itemSprite; 
    }

    public override void OnEnable()
    {
        base.OnEnable();

        waitTime = 1f;
    }

    public override void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Dictionary<Item, int> inventory = Traveller.Instance.inventory;
 
            if (inventory.ContainsKey(ItemDataBase.Instance.items[itemID]))
            {
                inventory[ItemDataBase.Instance.items[itemID]]++;
            }
            else
            {
                inventory.Add(ItemDataBase.Instance.items[itemID], 1);
            }

            Debug.Log($"{name} : InsertQueue");
            ObjectManager.Instance.InsertQueue(PoolType.Item, gameObject);
        }       
    }
}
