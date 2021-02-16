using UnityEngine;

public class ShopManager : MonoBehaviour
{
    [SerializeField] private GameObject buyPanel;
    [SerializeField] private GameObject sellPanel;
    [SerializeField] private ItemInventory ItemInventory;

    [ContextMenu("ASD")]
    public void Initialize()
    {
        for (int i = 0; i < sellPanel.transform.GetChild(0).transform.childCount; i++)
        {
            if (i < ItemInventory.Items.Count)
            {
                sellPanel.transform.GetChild(0).transform.GetChild(i).GetComponent<ItemSlot>().SetItemSlot(ItemInventory.Items[i]);
            }
            else
            {
                
            }
        }
    }
}
