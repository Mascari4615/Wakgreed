using UnityEngine;
using System.Collections.Generic;

public class Chef : NPC
{
    [SerializeField] private FoodInventory foodInventory;

    private void Start()
    {
        if (foodInventory.Items != null) foodInventory.Clear();
        ui = UIManager.Instance.restaurant.gameObject;

        List<Food> asd = new();
        foreach (var kav in DataManager.Instance.FoodDic)
        {
            asd.Add(kav.Value);
        }

        for (int i = 0; i < 3; i++)
        {
            int rand = Random.Range(0, asd.Count);
            foodInventory.Add(asd[rand]);
            asd.RemoveAt(rand);
        }
    } 
}
