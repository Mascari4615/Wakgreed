using UnityEngine;
using System.Collections;

public class ShopKeeper : NPC
{
    [SerializeField] private ItemInventoryUI itemInventoryUI_Sell;
    [SerializeField] private ItemInventoryUI itemInventoryUI_Buy;
    [SerializeField] private ItemInventory itemInventory;
    [SerializeField] private TreasureDataBuffer treasureDataBuffer;
    [SerializeField] private ShopKeeperItemInventory shopKeeperItemInventory;
    [SerializeField] private IntVariable nyang;
    [SerializeField] private GameEvent onNyangChange;
    [SerializeField] private GameObject needMoreNyang;

    private void Awake()
    {
        for (int i = 0; i < 5; i++)
        {
            shopKeeperItemInventory.Add(treasureDataBuffer.Items[Random.Range(0, treasureDataBuffer.Items.Length)]);
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
        // 버튼을 누름으로써 호출, 필요한 정보는 버튼을 누른 슬롯의 아이템, 인벤토리 (이건 있음)
        slot.gameObject.SetActive(false);
        itemInventory.Remove(slot.specialThing as Item);

        for (int i = 0; i < (slot.specialThing as Item).price / 10; i++)
        {
            ObjectManager.Instance.GetQueue(PoolType.Nyang10, transform);
        }

        for (int i = 0; i < (slot.specialThing as Item).price % 10; i++)
        {
            ObjectManager.Instance.GetQueue(PoolType.Nyang, transform);
        }
        
        // Debug.Log(itemInventory.Items.Count);
    }

    public void BuyItem(Slot slot)
    {
        if (nyang.RuntimeValue < (slot.specialThing as Item).price) 
        {
            StopCoroutine("NeedMoreNyang");
            StartCoroutine("NeedMoreNyang");
            return;
        }
        
        nyang.RuntimeValue -= (slot.specialThing as Item).price;
        onNyangChange.Raise();

        slot.gameObject.SetActive(false);
        shopKeeperItemInventory.Remove(slot.specialThing as Item);
        ObjectManager.Instance.GetQueue(PoolType.Item, transform).GetComponent<ItemGameObject>().SetItemGameObject((slot.specialThing as Item).ID, true);
    }

    private IEnumerator NeedMoreNyang()
    {
        needMoreNyang.SetActive(true);
        yield return new WaitForSeconds(1);
        needMoreNyang.SetActive(false);
    }
}
