using UnityEngine;

public class ShopKeeper : NPC
{
    [SerializeField] private ItemInventoryUI itemInventoryUI;
    [SerializeField] private ItemInventory itemInventory;
    
    public override void Interaction()
    { 
        base.Interaction();
        itemInventoryUI.Initialize();
    }

    public void SellItem(Slot slot)
    {
        // 버튼을 누름으로써 호출, 필요한 정보는 버튼을 누른 슬롯의 아이템, 인벤토리 (이건 있음)
        for (int i = 0; i < (slot.specialThing as Item).price / 10; i++)
        {
            ObjectManager.Instance.GetQueue(PoolType.Nyang10, transform);
        }

        for (int i = 0; i < (slot.specialThing as Item).price % 10; i++)
        {
            ObjectManager.Instance.GetQueue(PoolType.Nyang, transform);
        }

        slot.gameObject.SetActive(false);
        itemInventory.Remove(slot.specialThing as Item);
        Debug.Log(itemInventory.Items.Count);
    }
}
