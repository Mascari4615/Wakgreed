using UnityEngine;

public class ShopKeeper : NPC
{
    [SerializeField] private ItemInventoryUI itemInventoryUI_Sell;
    [SerializeField] private ItemInventoryUI itemInventoryUI_Buy;
    [SerializeField] private ItemInventory itemInventory;
    [SerializeField] private ItemDataBuffer ItemDataBuffer;
    [SerializeField] private ShopKeeperItemInventory shopKeeperItemInventory;
    [SerializeField] private IntVariable nyang;
    [SerializeField] private GameEvent onNyangChange;
    

    private void Awake()
    {
        for (int i = 0; i < 10; i++)
        {
            shopKeeperItemInventory.Add(ItemDataBuffer.Items[Random.Range(0, ItemDataBuffer.Items.Length)]);
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
        if (itemInventory.itemCountDic[(slot.specialThing as Item).ID] == 1) { slot.gameObject.SetActive(false); }
        itemInventory.Remove(slot.specialThing as Item);
        for (int i = 0; i < (slot.specialThing as Item).price / 10; i++) ObjectManager.Instance.GetQueue("Nyang10", transform);
        for (int i = 0; i < (slot.specialThing as Item).price % 10; i++) ObjectManager.Instance.GetQueue("Nyang", transform);
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
        onNyangChange.Raise();

        slot.gameObject.SetActive(false);
        shopKeeperItemInventory.Remove(slot.specialThing as Item);
        ObjectManager.Instance.GetQueue("Item", transform).GetComponent<ItemGameObject>().Initialize((slot.specialThing as Item).ID);
        itemInventoryUI_Buy.Initialize();
    }
}
