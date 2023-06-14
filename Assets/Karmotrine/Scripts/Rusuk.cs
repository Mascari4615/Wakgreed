using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Rusuk : NPC
{
    [SerializeField] protected FoodInventoryUI inventoryUI;
    [SerializeField] protected FoodDataBuffer foodDataBuffer;
    [SerializeField] protected WakgoodFoodInventory wakgoodFoodInventory;
    [SerializeField] protected IntVariable goldu;
    [SerializeField] protected int count = 8;
    private Slot tempSlot;

    protected override void Awake()
    {
        base.Awake();
        ResetInven();
    }

    public void ResetInven()
    {
        canOpenUI = true;
        List<Food> temp = foodDataBuffer.items.ToList();

        inventoryUI.NpcInventory.Clear();
        for (int i = 0; i < count; i++)
        {
            int random = Random.Range(0, temp.Count);
            inventoryUI.NpcInventory.Add(temp[random]);
            temp.RemoveAt(random);
        }
    }

    public void BuyFood(Slot slot)
    {
        tempSlot = slot;
        canOpenUI = false;
        customUI.SetActive(false);
        StartCoroutine(nameof(Shaking));
    }

    private IEnumerator Shaking()
    {
        cvm1.Priority = 200;
        cvm2.Priority = -100;
        GameManager.Instance.CinemachineTargetGroup.m_Targets[1].target = transform;
        GameManager.Instance.CinemachineTargetGroup.m_Targets[1].weight = 5;
        animator.SetBool("SAKING", true);
        yield return new WaitForSeconds(1.5f);
        animator.SetBool("SAKING", false);
        wakgoodFoodInventory.Add(tempSlot.SpecialThing as Food);
        DataManager.Instance.wgItemInven.Add(DataManager.Instance.ItemDic[57]);
        base.FocusOff();
    }
}