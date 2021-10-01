using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Restaurant : MonoBehaviour
{
    [SerializeField] private WakgoodFoodInventory wakgoodFoodInventory;
    [SerializeField] private IntVariable nyang;
    [HideInInspector] public RestaurantFoodInventory curRestaurantFoodInventory;

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
        curRestaurantFoodInventory.Remove(slot.specialThing as Food);
        wakgoodFoodInventory.Add(slot.specialThing as Food);
        GetComponent<FoodInventoryUI>().Initialize();
    }
}
