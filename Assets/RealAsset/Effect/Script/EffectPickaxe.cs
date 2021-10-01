using UnityEngine;

[CreateAssetMenu(fileName = "EffectPickaxe", menuName = "Effect/EffectPickaxe")]
public class EffectPickaxe : Effect
{
    [SerializeField] private GameEvent OnMonsterCollapse;

    private void GetOre(Transform transform)
    {
        ObjectManager.Instance.PopObject("Item", transform.position).GetComponent<ItemGameObject>().Initialize(400);
    }

    public override void _Effect()
    {
        OnMonsterCollapse.AddCollback(GetOre);
    }

    public override void Return()
    {
        OnMonsterCollapse.RemoveCollback(GetOre);
    }
}
