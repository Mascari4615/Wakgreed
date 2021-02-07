using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInventory : MonoBehaviour
{
    [SerializeField] private GameObject grid;
    
    public void Initialize()
    {
        List<Item> temp_inventory = new List<Item>();
        foreach (var item in TravellerController.Instance.traveller.inventory.Keys)
        {
            temp_inventory.Add(item);
        }
        for (int i = 0; i < temp_inventory.Count; i++)
        {
            grid.transform.GetChild(i).GetComponent<ItemSlot>().SetItemSlot(temp_inventory[i].sprite, TravellerController.Instance.traveller.inventory[temp_inventory[i]]);
        }
    }
}