using UnityEngine;
[CreateAssetMenu(fileName = "ChangeItemToItem", menuName = "Skill/ChangeItemToItem")]
public class ChangeItemToItem : Skill
{
    [SerializeField] private Item origin;
    [SerializeField] private int originCount;
    [SerializeField] private Item[] target;

    public override void Use(Weapon weapon)
    {
        if (DataManager.Instance.wakgoodItemInventory.Items.Find(x => x.id == origin.id))
        {
            if (DataManager.Instance.wakgoodItemInventory.itemCountDic[origin.id] >= originCount)
            {
                for (int i = 0; i < originCount; i++)
                    DataManager.Instance.wakgoodItemInventory.Remove(DataManager.Instance.ItemDic[origin.id]);

                int random = Random.Range(0, target.Length);
                DataManager.Instance.wakgoodItemInventory.Add(DataManager.Instance.ItemDic[target[random].id]);
                ObjectManager.Instance.PopObject("Effect_Hit", Wakgood.Instance.transform);
            }
        }
        else
        {
            ObjectManager.Instance.PopObject("AnimatedText", Wakgood.Instance.transform).GetComponent<AnimatedText>().SetText("재료가 부족합니다!", TextType.Critical);
        }
    }
}
