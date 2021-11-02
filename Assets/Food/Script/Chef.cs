using System.Linq;
using UnityEngine;

public class Chef : NPC
{
    [SerializeField] private FoodInventoryUI FoodInventoryUI;
    [SerializeField] private WakgoodFoodInventory wakgoodFoodInventory;
    [SerializeField] private RestaurantFoodInventory foodInventory;
    [SerializeField] private IntVariable nyang; 

    private void Awake()
    {
        for (int i = 0; i < 10; i++)
        {
            foodInventory.Add(DataManager.Instance.FoodDic.ElementAt(Random.Range(0, DataManager.Instance.FoodDic.Count)).Value);
        }
    }

    public void BuyFood(Slot slot)
    {
        if (nyang.RuntimeValue >= (slot.specialThing as Food).price)
        {
            nyang.RuntimeValue -= (slot.specialThing as Food).price;

            slot.gameObject.SetActive(false);

            foodInventory.Remove(slot.specialThing as Food);
            wakgoodFoodInventory.Add(slot.specialThing as Food);
            FoodInventoryUI.Initialize();
        }
        else
        {
            ObjectManager.Instance.PopObject("AnimatedText", Wakgood.Instance.transform.position).GetComponent<AnimatedText>().SetText("골드 부족!", TextType.Critical);
        }
    }
}