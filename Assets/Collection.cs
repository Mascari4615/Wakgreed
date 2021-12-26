using System.Collections.Generic;
using UnityEngine;

public class Collection : MonoBehaviour
{
    public static Collection Instance;

    [SerializeField] private Transform viewPort;
    [SerializeField] private List<Slot> bossSlots = new();
    [SerializeField] private List<Slot> monsterSlots = new();
    [SerializeField] private List<Slot> itemSlots = new();
    [SerializeField] private List<Slot> masterySlot = new();

    private void Awake()
    {
        Instance = this;

        for (int i = 0; i < DataManager.Instance.CurGameData.killedOnceBoss.Length; i++)
            if (DataManager.Instance.CurGameData.killedOnceBoss[i])
                bossSlots[i].SetSlot(DataManager.Instance.BossDic[i]);
        for (int i = 0; i < DataManager.Instance.CurGameData.killedOnceMonster.Length; i++)
            if (DataManager.Instance.CurGameData.killedOnceMonster[i])
                monsterSlots[i].SetSlot(DataManager.Instance.MonsterDic[i]);

        for (int i = 0; i < DataManager.Instance.ItemDic.Count; i++)
            if (DataManager.Instance.CurGameData.equipedOnceItem[i])
                itemSlots[i].SetSlot(DataManager.Instance.ItemDic[i]);

        for (int i = 0; i < DataManager.Instance.MasteryDic.Count; i++)
            if (DataManager.Instance.CurGameData.getOnceMastery[i])
                masterySlot[i].SetSlot(DataManager.Instance.MasteryDic[i]);
    }

    public void Collect(Monster monster, bool isBoss = false)
    {
        if (isBoss)
            bossSlots[monster.ID].SetSlot(monster);
        else
            monsterSlots[monster.ID].SetSlot(monster);
    }
    public void Collect(Item item) => itemSlots[item.id].SetSlot(item);
    public void Collect(Mastery mastery) => masterySlot[mastery.id].SetSlot(mastery);
}
