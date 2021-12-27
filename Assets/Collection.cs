using System.Collections.Generic;
using UnityEngine;

public class Collection : MonoBehaviour
{
    public static Collection Instance;

    [SerializeField] private Transform viewPort;
    [SerializeField] private List<Slot> bossSlots = new();
    [SerializeField] private List<Slot> monsterSlots = new();
    [SerializeField] private List<Slot> itemSlots = new();

    private void Awake()
    {
        Instance = this;

        for (int i = 0; i < DataManager.Instance.CurGameData.killedOnceBoss.Length; i++)
            if (DataManager.Instance.CurGameData.killedOnceBoss[i])
                bossSlots[i].SetSlot(DataManager.Instance.BossDic[i]);
            else
                bossSlots[i].toolTipTrigger.enabled = false;
        
        for (int i = 0; i < DataManager.Instance.CurGameData.killedOnceMonster.Length; i++)
            if (DataManager.Instance.CurGameData.killedOnceMonster[i])
                monsterSlots[i].SetSlot(DataManager.Instance.MonsterDic[i]);
            else
                monsterSlots[i].toolTipTrigger.enabled = false;

        for (int i = 0; i < DataManager.Instance.ItemDic.Count; i++)
            if (DataManager.Instance.CurGameData.equipedOnceItem[i])
                itemSlots[i].SetSlot(DataManager.Instance.ItemDic[i]);
            else
                itemSlots[i].toolTipTrigger.enabled = false;
    }

    public void Collect(Monster monster, bool isBoss = false)
    {
        if (isBoss)
            bossSlots[monster.ID].SetSlot(monster);
        else
            monsterSlots[monster.ID].SetSlot(monster);
    }
    public void Collect(Item item) => itemSlots[item.id].SetSlot(item);
}
