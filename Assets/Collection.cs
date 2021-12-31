using System.Collections.Generic;
using UnityEngine;

public class Collection : MonoBehaviour
{
    public static Collection Instance;

    [SerializeField] private List<Slot> monsterSlots = new();
    [SerializeField] private List<Slot> itemSlots = new();

    private void Awake()
    {
        Instance = this;

        for (int i = 0; i < monsterSlots.Count; i++)
        {
            if (i < DataManager.Instance.MonsterDic.Count)
            {
                monsterSlots[i].gameObject.SetActive(true);

                if (DataManager.Instance.CurGameData.killedOnceMonster[i])
                    monsterSlots[i].SetSlot(DataManager.Instance.MonsterDic[i]);
                else
                    monsterSlots[i].toolTipTrigger.enabled = false;
            }
            else
            {
                monsterSlots[i].gameObject.SetActive(false);
            }
        }

        for (int i = 0; i < itemSlots.Count; i++)
        {
            if (i < DataManager.Instance.ItemDic.Count)
            {
                itemSlots[i].gameObject.SetActive(true);

                if (DataManager.Instance.CurGameData.equipedOnceItem[i])
                    itemSlots[i].SetSlot(DataManager.Instance.ItemDic[i]);
                else
                    itemSlots[i].toolTipTrigger.enabled = false;
            }
            else
            {
                itemSlots[i].gameObject.SetActive(false);
            }
        }
    }

    public void Collect(Monster monster) => monsterSlots[monster.ID].SetSlot(monster);
    public void Collect(Item item) => itemSlots[item.id].SetSlot(item);
}
