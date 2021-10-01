using UnityEngine;

[CreateAssetMenu(fileName = "PickaxeUpgrade", menuName = "Skill/PickaxeUpgrade")]
public class PickaxeUpgrade : Skill
{
    public override void Use()
    {
        int stoneID = 400;
        if (DataManager.Instance.WakgoodItemInventory.Items.Find(x => x.ID == stoneID))
        {
            if (DataManager.Instance.WakgoodItemInventory.itemCountDic[stoneID] >= 5)
            {
                DataManager.Instance.WakgoodItemInventory.Remove(DataManager.Instance.ItemDic[stoneID]);
                DataManager.Instance.WakgoodItemInventory.Remove(DataManager.Instance.ItemDic[stoneID]);
                DataManager.Instance.WakgoodItemInventory.Remove(DataManager.Instance.ItemDic[stoneID]);
                DataManager.Instance.WakgoodItemInventory.Remove(DataManager.Instance.ItemDic[stoneID]);
                DataManager.Instance.WakgoodItemInventory.Remove(DataManager.Instance.ItemDic[stoneID]);
                Wakgood.Instance.SwitchWeapon(targetWeapon: DataManager.Instance.WeaponDic[Wakgood.Instance.curWeapon.ID + 1]);
            }
        }
    }
}
