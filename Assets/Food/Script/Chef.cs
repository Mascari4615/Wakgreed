using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class Chef : NPC
{
    [SerializeField] protected FoodInventoryUI inventoryUI;
    [SerializeField] protected FoodDataBuffer foodDataBuffer;
    [SerializeField] protected WakgoodFoodInventory wakgoodFoodInventory;
    [SerializeField] protected IntVariable goldu; 
    [SerializeField] protected int count = 8; 

    protected override void Awake()
    {
        base.Awake();

        List<Food> temp = foodDataBuffer.items.ToList();

        for (int i = 0; i < count; i++)
        {
            int random = Random.Range(0, temp.Count);
            inventoryUI.NpcInventory.Add(temp[random]);
            temp.RemoveAt(random);
        }
    }

    public virtual void BuyFood(Slot slot)
    {
        if (goldu.RuntimeValue >= (slot.SpecialThing as Food).price)
        {
            goldu.RuntimeValue -= (slot.SpecialThing as Food).price;

            slot.gameObject.SetActive(false);
            inventoryUI.NpcInventory.Remove(slot.SpecialThing as Food);
            wakgoodFoodInventory.Add(slot.SpecialThing as Food);
            RuntimeManager.PlayOneShot($"event:/SFX/UI/Test", transform.position);

        }
        else
        {
            ObjectManager.Instance.PopObject("AnimatedText", Wakgood.Instance.transform.position).GetComponent<AnimatedText>().SetText("골두 부족!", TextType.Critical);
            RuntimeManager.PlayOneShot($"event:/SFX/UI/No", transform.position);

        }
    }
}