using System.Collections.Generic;
using UnityEngine;

public abstract class InventoryUI<T> : MonoBehaviour
{
    [SerializeField] private RunTimeSet<T> Inventory;
    [HideInInspector] public List<T> NpcInventory;
    [HideInInspector] public List<Slot> slots = new();
    [HideInInspector] public bool temp;

    private void Awake()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            slots.Add(transform.GetChild(i).GetComponent<Slot>());
        }
    }

    private void OnEnable()
    {
        Initialize();
    }

    public void Initialize()
    {
        if (slots.Count == 0)
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                slots.Add(transform.GetChild(i).GetComponent<Slot>());
            }
        }

        Dictionary<Item, int> itemDic = new();
        if (temp)
        {
            foreach (Item item in NpcInventory as List<Item>)
            {
                if (itemDic.ContainsKey(item))
                {
                    itemDic[item]++;
                }
                else
                {
                    itemDic.Add(item, 1);
                }
            }

            int a = 0;
            foreach (KeyValuePair<Item, int> item in itemDic)
            {
                slots[a++].SetSlot(item.Key, item.Value);
            }

            for (int i = 0; i < transform.childCount; i++)
            {
                if (i < itemDic.Count)
                {
                    slots[i].gameObject.SetActive(true);
                }
                else
                {
                    slots[i].gameObject.SetActive(false);
                }
            }
        }
        else
        {
            if (Inventory != null)
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    if (i < Inventory.Items.Count)
                    {
                        slots[i].SetSlot(Inventory.Items[i] as SpecialThing);
                        slots[i].gameObject.SetActive(true);
                    }
                    else
                    {
                        slots[i].gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                for (int i = 0; i < transform.childCount; i++)
                {
                    if (i < NpcInventory.Count)
                    {
                        slots[i].SetSlot(NpcInventory[i] as SpecialThing);
                        slots[i].gameObject.SetActive(true);
                    }
                    else
                    {
                        slots[i].gameObject.SetActive(false);
                    }
                }
            }
        }
    }
}