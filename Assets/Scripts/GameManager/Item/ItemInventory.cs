using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInventory : MonoBehaviour
{
    public GameObject grid;
    public void Initialize()
    {
        for (int i = 0; i < GameManager.Instance.player.inventory.Count; i++)
        {
            grid.transform.GetChild(i).GetComponent<ItemSlot>().SetItemSlot(GameManager.Instance.player.inventory[i].itemSprite, GameManager.Instance.player.inventory[i].itemCount);
        }
    }
}