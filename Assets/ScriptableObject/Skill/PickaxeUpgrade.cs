using UnityEngine;

[CreateAssetMenu(fileName = "PickaxeUpgrade", menuName = "Skill/PickaxeUpgrade")]
public class PickaxeUpgrade : Skill
{
    public override void Use()
    {
        if (DataManager.Instance.ItemInventory.Items.Find(x => x.ID == 100))
        {
            if (DataManager.Instance.ItemInventory.itemCountDic[100] >= 5)
            {
                DataManager.Instance.ItemInventory.Remove(DataManager.Instance.ItemDic[100]);
                DataManager.Instance.ItemInventory.Remove(DataManager.Instance.ItemDic[100]);
                DataManager.Instance.ItemInventory.Remove(DataManager.Instance.ItemDic[100]);
                DataManager.Instance.ItemInventory.Remove(DataManager.Instance.ItemDic[100]);
                DataManager.Instance.ItemInventory.Remove(DataManager.Instance.ItemDic[100]);
                TravellerController.Instance.SwitchWeapon(targetWeapon: DataManager.Instance.WeaponDic[TravellerController.Instance.curWeapon.ID + 1]);
            }
        }
    }
}
