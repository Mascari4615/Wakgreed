using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInventory : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private GameObject grid;
    
    public void Initialize()
    {
        for (int i = 0; i < inventory.Items.Count; i++)
        {
            grid.transform.GetChild(i).GetComponent<ItemSlot>().SetItemSlot(inventory.Items[i].sprite, inventory.Items[i].count);
        }
    }
}