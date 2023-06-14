using UnityEngine;

[CreateAssetMenu(fileName = "EffectGetItem", menuName = "Effect/GetItem")]
public class EffectGetItem : Effect
{
    [SerializeField] private int id;

    public override void _Effect()
    {
        DataManager.Instance.wgItemInven.Add(DataManager.Instance.ItemDic[id]);
    }

    public override void Return()
    {
        // DataManager.Instance.wgItemInven.Remove(DataManager.Instance.ItemDic[id]);
    }
}