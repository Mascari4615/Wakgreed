using System.Linq;
using UnityEngine;

public class ShopKeeper : NPC
{
    [SerializeField] private ShopKeeperItemInventory itemInventory;
    [SerializeField] private ItemInventoryUI itemInventoryUI_Sell;
    [SerializeField] private ItemInventoryUI itemInventoryUI_Buy;
    [SerializeField] private IntVariable nyang;

    private void Awake()
    {
        for (int i = 0; i < 10; i++)
        {
            itemInventory.Add(DataManager.Instance.ItemDic.ElementAt(Random.Range(0, DataManager.Instance.ItemDic.Count)).Value);
        }
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
        if (nyang.RuntimeValue >= (slot.specialThing as Item).price)
        {
            nyang.RuntimeValue -= (slot.specialThing as Item).price;

            slot.gameObject.SetActive(false);

            itemInventory.Remove(slot.specialThing as Item);
            ObjectManager.Instance.PopObject("ItemGameObject", transform).GetComponent<ItemGameObject>().Initialize((slot.specialThing as Item).ID);
            itemInventoryUI_Buy.Initialize();
        }
        else
        {
            ObjectManager.Instance.PopObject("AnimatedText", Wakgood.Instance.transform.position).GetComponent<AnimatedText>().SetText("∞ÒµÂ ∫Œ¡∑!", TextType.Critical);
        }
    }
}
