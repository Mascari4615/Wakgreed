using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGameObject : MonoBehaviour
{
    public int itemID;
    public void SetItem(Item item)
    {
        itemID = item.itemID;
        this.name = item.itemName;
        GetComponent<SpriteRenderer>().sprite = item.itemSprite; 
    }
}
