using UnityEngine;

public abstract class InventoryUI<T> : MonoBehaviour
{
    [SerializeField] private RunTimeSet<T> Inventory;
    [SerializeField] private GameObject grid;
    
    public void Initialize()
    {
        for (int i = 0; i < grid.transform.childCount; i++)
        {
            if (i < Inventory.Items.Count)
            {
                grid.transform.GetChild(i).GetComponent<Slot>().SetSlot(Inventory.Items[i] as SpecialThing);
                grid.transform.GetChild(i).gameObject.SetActive(true);
            }
            else
            {
                grid.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }
}