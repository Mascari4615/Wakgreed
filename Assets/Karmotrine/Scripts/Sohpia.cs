using FMODUnity;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Sohpia : NPC
{
    [SerializeField] private ItemInventoryUI itemInventoryUI_Buy;
    [SerializeField] private IntVariable nyang;

    protected override void Awake()
    {
        base.Awake();
        ResetInven();
    }

    public void ResetInven()
    {
        List<Item> temp = DataManager.Instance.itemDataBuffer.items.ToList();
        itemInventoryUI_Buy.NpcInventory.Clear();
        for (int i = 0; i < 5; i++)
        {
            int random = Random.Range(0, temp.Count);
            itemInventoryUI_Buy.NpcInventory.Add(temp[random]);
            temp.RemoveAt(random);
        }
    }

    public override void FocusOff()
    {
        base.FocusOff();
        DataManager.Instance.CurGameData.talkedOnceNPC[ID] = true;
        DataManager.Instance.CurGameData.rescuedNPC[ID] = true;
    }

    public void BuyItem(Slot slot)
    {
        if (nyang.RuntimeValue >= (slot.SpecialThing as Item).price)
        {
            nyang.RuntimeValue -= (slot.SpecialThing as Item).price;

            slot.gameObject.SetActive(false);

            itemInventoryUI_Buy.NpcInventory.Remove(slot.SpecialThing as Item);
            ObjectManager.Instance.PopObject("ItemGameObject", transform).GetComponent<ItemGameObject>()
                .Initialize((slot.SpecialThing as Item).id);
            RuntimeManager.PlayOneShot("event:/SFX/UI/Test", transform.position);
            itemInventoryUI_Buy.Initialize();
        }
        else
        {
            RuntimeManager.PlayOneShot("event:/SFX/UI/No", transform.position);
            ObjectManager.Instance.PopObject("AnimatedText", Wakgood.Instance.transform.position)
                .GetComponent<AnimatedText>().SetText("골두 부족!", Color.yellow);
        }
    }
}