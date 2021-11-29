using UnityEngine;

[CreateAssetMenu(fileName = "PickaxeUpgrade", menuName = "Skill/PickaxeUpgrade")]
public class PickaxeUpgrade : Skill
{
    private const int stoneID = 400;

    public override void Use(Weapon weapon)
    {       
        if (DataManager.Instance.wakgoodItemInventory.Items.Find(x => x.id == stoneID))
        {
            if (DataManager.Instance.wakgoodItemInventory.itemCountDic[stoneID] >= 5)
            {
                DataManager.Instance.wakgoodItemInventory.Remove(DataManager.Instance.ItemDic[stoneID]);
                DataManager.Instance.wakgoodItemInventory.Remove(DataManager.Instance.ItemDic[stoneID]);
                DataManager.Instance.wakgoodItemInventory.Remove(DataManager.Instance.ItemDic[stoneID]);
                DataManager.Instance.wakgoodItemInventory.Remove(DataManager.Instance.ItemDic[stoneID]);
                DataManager.Instance.wakgoodItemInventory.Remove(DataManager.Instance.ItemDic[stoneID]);
                Wakgood.Instance.SwitchWeapon(Wakgood.Instance.CurWeaponNumber, DataManager.Instance.WeaponDic[Wakgood.Instance.CurWeapon.id + 1]);
            }
        }
        else
        {
            ObjectManager.Instance.PopObject("AnimatedText", Wakgood.Instance.transform).GetComponent<AnimatedText>().SetText("재료가 부족합니다!", TextType.Critical);
        }
    }
}
