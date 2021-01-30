using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInventory : MonoBehaviour
{
    public GameObject grid;
    public void Initialize()
    {
        List<Item> temp_inventory = new List<Item>();
        foreach (var item in Traveller.Instance.inventory.Keys)
        {
            temp_inventory.Add(item);
        }
        for (int i = 0; i < temp_inventory.Count; i++)
        {
            grid.transform.GetChild(i).GetComponent<ItemSlot>().SetItemSlot(temp_inventory[i].itemSprite, Traveller.Instance.inventory[temp_inventory[i]]);
        }
    }
}