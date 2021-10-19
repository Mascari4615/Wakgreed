using UnityEngine;

[CreateAssetMenu(fileName = "PickaxeUpgrade", menuName = "Skill/PickaxeUpgrade")]
public class PickaxeUpgrade : Skill
{
    private const int stoneID = 400;

    public override void Use()
    {       
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
        else
        {
            ObjectManager.Instance.PopObject("AnimatedText", Wakgood.Instance.transform).GetComponent<AnimatedText>().SetText("재료가 부족합니다!", TextType.Critical);
        }
    }
}
