using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private Inventory inventory;
    [SerializeField] private GameObject grid;
    
    public void Initialize()
    {
        for (int i = 0; i < grid.transform.childCount; i++)
        {
            if (i < inventory.Items.Count)
            {
                grid.transform.GetChild(i).GetComponent<ItemSlot>().SetItemSlot(inventory.Items[i]);
            }
            else
            {
                grid.transform.GetChild(i).GetComponent<ItemSlot>().toolTipTrigger.enabled = false;
            }
        }
    }
}