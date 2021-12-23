using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Collection : MonoBehaviour
{
    public static Collection Instance;

    [SerializeField] private Transform viewPort;
    private List<Image> bossSlots = new();
    private List<Image> monsterSlots = new();
    private List<Image> itemSlots = new();
    private List<Image> masterySlot = new();

    private void Awake()
    {
        Instance = this;

        int i = 0;

        for (i = 0; i < viewPort.childCount; i++)
        {
            // i item mastery
            Transform temp = viewPort.GetChild(i);

            for (int j = 0; j < temp.childCount; j++)
            {
                switch (i)
                {
                    case 0:
                        itemSlots.Add(temp.GetChild(j).GetComponent<Image>());
                        break;
                    case 1:
                        masterySlot.Add(temp.GetChild(j).GetComponent<Image>());
                        break;
                    case 2:
                        monsterSlots.Add(temp.GetChild(j).GetComponent<Image>());
                        break;
                    case 3:
                        bossSlots.Add(temp.GetChild(j).GetComponent<Image>());
                        break;
                }

                temp.GetChild(j).gameObject.SetActive(false);
            }
        }


        i = 0;
        foreach (var item in DataManager.Instance.BossDic)
        {
            var value = item.Value;
            bossSlots[i++].sprite = item.Value.GetComponent<SpriteRenderer>().sprite;
        }

        i = 0;
        foreach (var item in DataManager.Instance.MonsterDic)
        {
            var value = item.Value;
            monsterSlots[i++].sprite = item.Value.GetComponent<SpriteRenderer>().sprite;
        }

        i = 0;
        foreach (var item in DataManager.Instance.ItemDic)
        {
            var value = item.Value;
            itemSlots[i++].sprite = item.Value.sprite;
        }

        i = 0;
        foreach (var item in DataManager.Instance.MasteryDic)
        {
            var value = item.Value;
            masterySlot[i++].sprite = item.Value.sprite;
        }

        for (i = 0; i < DataManager.Instance.CurGameData.killedOnceBoss.Length; i++)
            bossSlots[i].gameObject.SetActive(DataManager.Instance.CurGameData.killedOnceBoss[i]);

        for (i = 0; i < DataManager.Instance.CurGameData.killedOnceMonster.Length; i++)
            monsterSlots[i].gameObject.SetActive(DataManager.Instance.CurGameData.killedOnceMonster[i]);

        for (i = 0; i < DataManager.Instance.ItemDic.Count; i++)
            itemSlots[i].gameObject.SetActive(DataManager.Instance.CurGameData.equipedOnceItem[i]);

        for (i = 0; i < DataManager.Instance.MasteryDic.Count; i++)
            masterySlot[i].gameObject.SetActive(DataManager.Instance.CurGameData.getOnceMastery[i]);
    }

    public void Collect(Monster monster, bool isBoss = false)
    {
        if (isBoss)
            bossSlots[monster.ID].gameObject.SetActive(true);
        else
            monsterSlots[monster.ID].gameObject.SetActive(true);
    }

    public void Collect(Item item) => itemSlots[item.id].gameObject.SetActive(true);
    public void Collect(Mastery mastery) => masterySlot[mastery.id].gameObject.SetActive(true);
}
