using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private ItemInventory ItemInventory;
    [SerializeField] private GameObject grid;
    
    public void Initialize()
    {
        for (int i = 0; i < grid.transform.childCount; i++)
        {
            if (i < ItemInventory.Items.Count)
            {
                grid.transform.GetChild(i).GetComponent<ItemSlot>().SetItemSlot(ItemInventory.Items[i]);
            }
            else
            {
                grid.transform.GetChild(i).GetComponent<ItemSlot>().toolTipTrigger.enabled = false;
            }
        }
    }
}