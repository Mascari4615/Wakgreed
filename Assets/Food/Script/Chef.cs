using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Chef : NPC
{
    [SerializeField] protected FoodDataBuffer foodDataBuffer;
    [SerializeField] protected WakgoodFoodInventory wakgoodFoodInventory;
    [SerializeField] protected RestaurantFoodInventory foodInventory;
    [SerializeField] protected IntVariable goldu; 

    protected override void Awake()
    {
        base.Awake();

        List<Food> temp = foodDataBuffer.items.ToList();

        for (int i = 0; i < 8; i++)
        {
            int random = Random.Range(0, temp.Count);
            foodInventory.Add(temp[random]);
            temp.RemoveAt(random);
        }
    }

    public virtual void BuyFood(Slot slot)
    {
        if (goldu.RuntimeValue >= (slot.specialThing as Food).price)
        {
            goldu.RuntimeValue -= (slot.specialThing as Food).price;

            slot.gameObject.SetActive(false);

            foodInventory.Remove(slot.specialThing as Food);
            wakgoodFoodInventory.Add(slot.specialThing as Food);
        }
        else
        {
            ObjectManager.Instance.PopObject("AnimatedText", Wakgood.Instance.transform.position).GetComponent<AnimatedText>().SetText("골드 부족!", TextType.Critical);
        }
    }
}