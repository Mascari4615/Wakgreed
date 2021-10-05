using UnityEngine;
using System.Collections.Generic;

public class RestaurantOwner : NPC
{
    [SerializeField] private FoodInventoryUI FoodInventoryUI;
/*
    private void Start()
    {
        FoodInventoryUI = UIManager.Instance.FoodInventoryUI;
        ui = FoodInventoryUI.gameObject;

        List<Food> asd = new();
        foreach (var kav in DataManager.Instance.FoodDic)
        {
            asd.Add(kav.Value);
        }

        for (int i = 0; i < 3; i++)
        {
            int rand = Random.Range(0, asd.Count);

            asd.RemoveAt(rand);
        }
    }

    public override void Interaction()
    {
        base.Interaction();
        FoodInventoryUI.Initialize();
    }
    */
}
