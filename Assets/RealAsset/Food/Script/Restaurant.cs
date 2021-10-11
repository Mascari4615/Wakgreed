using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Restaurant : MonoBehaviour
{
    [SerializeField] private WakgoodFoodInventory wakgoodFoodInventory;
    [SerializeField] private RestaurantFoodInventory RestaurantFoodInventory;
    [SerializeField] private IntVariable nyang;
    [SerializeField] private FoodInventoryUI FoodInventoryUI;

    private void OnEnable()
    {
        FoodInventoryUI.Initialize();
    }

    public void BuyFood(Slot slot)
    {
        if (nyang.RuntimeValue < (slot.specialThing as Food).price)
        {
            UIManager.Instance.StopCoroutine("NeedMoreNyang");
            UIManager.Instance.StartCoroutine("NeedMoreNyang");
            return;
        }

        nyang.RuntimeValue -= (slot.specialThing as Food).price;

        slot.gameObject.SetActive(false);

        RestaurantFoodInventory.Remove(slot.specialThing as Food);
        wakgoodFoodInventory.Add(slot.specialThing as Food);
        FoodInventoryUI.Initialize();
    }
}
