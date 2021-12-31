using UnityEngine;
[CreateAssetMenu(fileName = "MasteryInventory", menuName = "GameSystem/RunTimeSet/MasteryInventory")]
public class MasteryInventory : RunTimeSet<Mastery>
{
    [SerializeField] private GameEvent MasterySelect;

    public override void Add(Mastery item)
    {
        if (!Items.Contains(item))
        {
            Items.Add(item);
            item.OnEquip();
            MasterySelect.Raise();
        }
    }

    public override void Remove(Mastery item)
    {
        if (Items.Contains(item))
        {
            item.OnRemove();
            Items.Remove(item);
        }
        else Debug.LogWarning($"RunTimeSet<Item> : �������� �ʴ� ������ ���� �õ�, {item.name}");
    }

    public override void Clear()
    {
        if (Items == null) return;
        int itemsCount = Items.Count;
        for (int i = 0; i < itemsCount; i++)
        {
            Remove(Items[0]);
        }
        Items.Clear();
    }
}