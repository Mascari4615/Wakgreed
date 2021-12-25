using UnityEngine;
using System.Collections.Generic;

public abstract class InventoryUI<T> : MonoBehaviour
{
    [SerializeField] private RunTimeSet<T> Inventory;
    public List<T> NpcInventory;
    private List<Slot> slots = new();

    private void Awake()
    {
        for (int i = 0; i < transform.childCount; i++)
            slots.Add(transform.GetChild(i).GetComponent<Slot>());
    }

    private void OnEnable() => Initialize();

    public void Initialize()
    {
        if (slots.Count == 0)
            for (int i = 0; i < transform.childCount; i++)
                slots.Add(transform.GetChild(i).GetComponent<Slot>());

        if (Inventory != null)
            for (int i = 0; i < transform.childCount; i++)
            {
                if (i < Inventory.Items.Count)
                {
                    slots[i].SetSlot(Inventory.Items[i] as SpecialThing);
                    slots[i].gameObject.SetActive(true);
                }
                else
                    slots[i].gameObject.SetActive(false);
            }
        else
            for (int i = 0; i < transform.childCount; i++)
            {
                if (i < NpcInventory.Count)
                {
                    slots[i].SetSlot(NpcInventory[i] as SpecialThing);
                    slots[i].gameObject.SetActive(true);
                }
                else
                    slots[i].gameObject.SetActive(false);
            }
    }
}