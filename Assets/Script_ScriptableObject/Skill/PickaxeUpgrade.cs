using UnityEngine;

[CreateAssetMenu(fileName = "PickaxeUpgrade", menuName = "Skill/PickaxeUpgrade")]
public class PickaxeUpgrade : Skill
{
    public override void Use()
    {
        if (DataManager.Instance.WakgoodItemInventory.Items.Find(x => x.ID == 100))
        {
            if (DataManager.Instance.WakgoodItemInventory.itemCountDic[100] >= 5)
            {
                DataManager.Instance.WakgoodItemInventory.Remove(DataManager.Instance.ItemDic[100]);
                DataManager.Instance.WakgoodItemInventory.Remove(DataManager.Instance.ItemDic[100]);
                DataManager.Instance.WakgoodItemInventory.Remove(DataManager.Instance.ItemDic[100]);
                DataManager.Instance.WakgoodItemInventory.Remove(DataManager.Instance.ItemDic[100]);
                DataManager.Instance.WakgoodItemInventory.Remove(DataManager.Instance.ItemDic[100]);
                Wakgood.Instance.SwitchWeapon(targetWeapon: DataManager.Instance.WeaponDic[Wakgood.Instance.curWeapon.ID + 1]);
            }
        }
    }
}
