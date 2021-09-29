using UnityEngine;

[CreateAssetMenu(fileName = "EffectPickaxe", menuName = "Effect/EffectPickaxe")]
public class EffectPickaxe : Effect
{
    [SerializeField] private GameEvent OnMonsterCollapse;

    private void GetOre(Transform transform)
    {
        //Debug.Log($"{name} : GetOre");
        ObjectManager.Instance.PopObject("Item", transform.position).GetComponent<ItemGameObject>().Initialize(400);
    }

    public override void _Effect()
    {
        //Debug.Log($"{name} : Effect");
        OnMonsterCollapse.AddCollback(GetOre);
    }

    public override void Return()
    {
        //Debug.Log($"{name} : Return");
        OnMonsterCollapse.RemoveCollback(GetOre);
    }
}
