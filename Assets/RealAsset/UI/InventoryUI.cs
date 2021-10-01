using UnityEngine;

public abstract class InventoryUI<T> : MonoBehaviour
{
    [SerializeField] private RunTimeSet<T> Inventory;

    private void Awake()
    {
        Initialize();
    }
    
    public void Initialize()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (i < Inventory.Items.Count)
            {
                transform.GetChild(i).GetComponent<Slot>().SetSlot(Inventory.Items[i] as SpecialThing);
                transform.GetChild(i).gameObject.SetActive(true);
            }
            else
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }
}