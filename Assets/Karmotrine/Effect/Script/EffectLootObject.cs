using UnityEngine;

[CreateAssetMenu(fileName = "EffectLootObject", menuName = "Effect/LootObject")]
public class EffectLootObject : Effect
{
    [SerializeField] [Range(0f, 100f)] private float percentage = 1.0f;
    [SerializeField] private GameEvent OnMonsterCollapse;
    [SerializeField] private GameObject prefab;

    private void DropItem(Transform transform)
    {
        int random = Random.Range(1, 100 + 1);
        if (percentage >= random)
        {
            ObjectManager.Instance.PopObject(prefab.name, transform.position);
        }
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