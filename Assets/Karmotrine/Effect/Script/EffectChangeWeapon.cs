using UnityEngine;

[CreateAssetMenu(fileName = "EffectChangeWeapon", menuName = "Effect/ChangeWeapon")]
public class EffectChangeWeapon : Effect
{
    [SerializeField] private int id;

    public override void _Effect()
    {
        Wakgood.Instance.SwitchWeapon(Wakgood.Instance.CurWeaponNumber, DataManager.Instance.WeaponDic[id]);
    }

    public override void Return()
    {
    }
}