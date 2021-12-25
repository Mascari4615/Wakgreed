using UnityEngine;

[CreateAssetMenu(fileName = "PickaxeUpgrade", menuName = "Skill/PickaxeUpgrade")]
public class PickaxeUpgrade : Skill
{
    [SerializeField] private Item stone;

    public override void Use(Weapon weapon)
    {       
        if (DataManager.Instance.wakgoodItemInventory.Items.Find(x => x.id == stone.id))
        {
            if (DataManager.Instance.wakgoodItemInventory.itemCountDic[stone.id] >= 5)
            {
                DataManager.Instance.wakgoodItemInventory.Remove(DataManager.Instance.ItemDic[stone.id]);
                DataManager.Instance.wakgoodItemInventory.Remove(DataManager.Instance.ItemDic[stone.id]);
                DataManager.Instance.wakgoodItemInventory.Remove(DataManager.Instance.ItemDic[stone.id]);
                DataManager.Instance.wakgoodItemInventory.Remove(DataManager.Instance.ItemDic[stone.id]);
                DataManager.Instance.wakgoodItemInventory.Remove(DataManager.Instance.ItemDic[stone.id]);
                Wakgood.Instance.SwitchWeapon(Wakgood.Instance.CurWeaponNumber, DataManager.Instance.WeaponDic[Wakgood.Instance.Weapon[Wakgood.Instance.CurWeaponNumber].id + 1]);
                ObjectManager.Instance.PopObject("AnimatedText", Wakgood.Instance.transform).GetComponent<AnimatedText>().SetText("업그레이드 성공!", Color.blue);
            }
        }
        else
        {
            ObjectManager.Instance.PopObject("AnimatedText", Wakgood.Instance.transform).GetComponent<AnimatedText>().SetText("재료가 부족합니다!", TextType.Critical);
        }
    }
}
