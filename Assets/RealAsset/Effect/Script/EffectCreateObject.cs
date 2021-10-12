using UnityEngine;

[CreateAssetMenu(fileName = "EffectCreateObject", menuName = "Effect/CreateObject")]
public class EffectCreateObject : Effect
{
    [SerializeField] private GameObject prefab;
    private GameObject instance;

    public override void _Effect()
    {
        instance = Instantiate(prefab, Wakgood.Instance.transform.position, Quaternion.identity);
    }

    public override void Return()
    {
        Destroy(instance);
    }
}
