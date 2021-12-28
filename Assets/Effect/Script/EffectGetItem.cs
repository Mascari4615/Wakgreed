using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "EffectGetItem", menuName = "Effect/GetItem")]

public class EffectGetItem : Effect
{
    [SerializeField] int id;
    public override void _Effect()
    {
        DataManager.Instance.wakgoodItemInventory.Add(DataManager.Instance.ItemDic[id]);
    }
    public override void Return()
    {
        DataManager.Instance.wakgoodItemInventory.Remove(DataManager.Instance.ItemDic[id]);
    }
}
