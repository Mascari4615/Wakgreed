using UnityEngine;


[CreateAssetMenu(fileName = "WakgoodFoodInventory", menuName = "GameSystem/RunTimeSet/WakgoodFoodInventory")]
public class WakgoodFoodInventory : FoodInventory
{
    [SerializeField] private FoodVariable LastAtedFood;
    [SerializeField] private GameEvent OnEatFood;

    public override void Add(Food item)
    {
        base.Add(item);
        item.OnEquip();
        LastAtedFood.RuntimeValue = item;
        OnEatFood.Raise();
    }

    public override void Remove(Food item)
    {
        base.Remove(item);
        item.OnRemove();
    }
}
