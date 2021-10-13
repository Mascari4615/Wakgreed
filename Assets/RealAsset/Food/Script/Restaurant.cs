using UnityEngine;

public class Restaurant : MonoBehaviour
{
    [SerializeField] private WakgoodFoodInventory wakgoodFoodInventory;
    [SerializeField] private RestaurantFoodInventory RestaurantFoodInventory;
    [SerializeField] private IntVariable nyang;
    [SerializeField] private FoodInventoryUI FoodInventoryUI;

    public void BuyFood(Slot slot)
    {
        if (nyang.RuntimeValue >= (slot.specialThing as Food).price)
        {
            nyang.RuntimeValue -= (slot.specialThing as Food).price;

            slot.gameObject.SetActive(false);

            RestaurantFoodInventory.Remove(slot.specialThing as Food);
            wakgoodFoodInventory.Add(slot.specialThing as Food);
            FoodInventoryUI.Initialize();          
        }
        else
        {
            ObjectManager.Instance.PopObject("AnimatedText", Wakgood.Instance.transform.position).GetComponent<AnimatedText>().SetText("∞ÒµÂ ∫Œ¡∑!", TextType.Critical);
        }     
    }
}
