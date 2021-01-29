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
            List<Item> inventory = GameManager.Instance.player.inventory;

            for (int i = 0; i < inventory.Count; i++)
            {
                if (inventory[i] == GameManager.Instance.itemBuffer.items[itemID])
                {
                    inventory[i].itemCount++;
                    return;
                }
            }
            inventory.Add(GameManager.Instance.itemBuffer.items[itemID]);
            GameManager.Instance.itemBuffer.items[itemID].Effect();

            print("asd");
            ObjectManager.Instance.InsertQueue(PoolType.Item, gameObject);
            print("wer");
        }       
    }
}
