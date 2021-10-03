using UnityEngine;

[CreateAssetMenu(fileName = "EffectLootObject", menuName = "Effect/LootObject")]
public class EffectLootObject : Effect
{
    [SerializeField] private GameEvent OnMonsterCollapse;
    [SerializeField] private GameObject prefab;

    private void DropItem(Transform transform)
    {
        ObjectManager.Instance.PopObject(prefab.name, transform.position);
    }

    public override void _Effect()
    {
        OnMonsterCollapse.AddCollback(DropItem);
    }

    public override void Return()
    {
        OnMonsterCollapse.RemoveCollback(DropItem);
    }
}
