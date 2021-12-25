using System.Linq;
using UnityEngine;

public class ShopKeeper : NPC
{
    [SerializeField] private ItemInventoryUI itemInventoryUI_Buy;
    [SerializeField] private IntVariable nyang;

    protected override void Awake()
    {
        base.Awake();

        for (int i = 0; i < 8; i++)
            itemInventoryUI_Buy.NpcInventory.Add(DataManager.Instance.ItemDic.ElementAt(Random.Range(0, DataManager.Instance.ItemDic.Count)).Value);
    }

    public void BuyItem(Slot slot)
    {
        if (nyang.RuntimeValue >= (slot.specialThing as Item).price)
        {
            nyang.RuntimeValue -= (slot.specialThing as Item).price;

            slot.gameObject.SetActive(false);

            itemInventoryUI_Buy.NpcInventory.Remove(slot.specialThing as Item);
            ObjectManager.Instance.PopObject("ItemGameObject", transform).GetComponent<ItemGameObject>().Initialize((slot.specialThing as Item).id);
            itemInventoryUI_Buy.Initialize();
        }
        else
        {
            ObjectManager.Instance.PopObject("AnimatedText", Wakgood.Instance.transform.position).GetComponent<AnimatedText>().SetText("골두 부족!", TextType.Critical);
        }
    }
}
