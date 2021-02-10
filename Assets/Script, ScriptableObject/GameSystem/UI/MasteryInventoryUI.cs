using UnityEngine;

public class MasteryInventoryUI : MonoBehaviour
{
    [SerializeField] private MasteryInventory masteryInventory;
    [SerializeField] private GameObject grid;
    
    public void Initialize()
    {
        for (int i = 0; i < grid.transform.childCount; i++)
        {
            if (i < masteryInventory.Items.Count)
            {
                grid.transform.GetChild(i).GetComponent<MasterySlot>().SetMasterySlot(masteryInventory.Items[i]);
            }
            else
            {
                grid.transform.GetChild(i).GetComponent<MasterySlot>().toolTipTrigger.enabled = false;
            }
        }
    }
}