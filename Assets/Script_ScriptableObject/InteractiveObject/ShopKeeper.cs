using UnityEngine;
using System.Linq;

public class ShopKeeper : NPC
{
    [SerializeField] private ItemInventoryUI itemInventoryUI_Sell;
    [SerializeField] private ItemInventoryUI itemInventoryUI_Buy;
    [SerializeField] private ShopKeeperItemInventory ShopKeeperItemInventory;
    [SerializeField] private IntVariable nyang;

    private void Start()
    {
        for (int i = 0; i < 10; i++)
        {
            ShopKeeperItemInventory.Add(DataManager.Instance.ItemDic.ElementAt(Random.Range(0, DataManager.Instance.ItemDic.Count)).Value);
        }
    }

    public override void Interaction()
    {
        base.Interaction();
        itemInventoryUI_Sell.Initialize();
        itemInventoryUI_Buy.Initialize();
    }

    public void SellItem(Slot slot)
    {
        if (DataManager.Instance.WakgoodItemInventory.itemCountDic[(slot.specialThing as Item).ID] == 1) { slot.gameObject.SetActive(false); }
        DataManager.Instance.WakgoodItemInventory.Remove(slot.specialThing as Item);
        for (int i = 0; i < (slot.specialThing as Item).price / 10; i++) ObjectManager.Instance.PopObject("Nyang10", transform);
        for (int i = 0; i < (slot.specialThing as Item).price % 10; i++) ObjectManager.Instance.PopObject("Nyang", transform);
        itemInventoryUI_Sell.Initialize();
    }

    public void BuyItem(Slot slot)
    {
        if (nyang.RuntimeValue < (slot.specialThing as Item).price)
        {
            UIManager.Instance.StopCoroutine("NeedMoreNyang");
            UIManager.Instance.StartCoroutine("NeedMoreNyang");
            return;
        }

        nyang.RuntimeValue -= (slot.specialThing as Item).price;

        slot.gameObject.SetActive(false);
        ShopKeeperItemInventory.Remove(slot.specialThing as Item);
        ObjectManager.Instance.PopObject("Item", transform).GetComponent<ItemGameObject>().Initialize((slot.specialThing as Item).ID);
        itemInventoryUI_Buy.Initialize();
    }
}
