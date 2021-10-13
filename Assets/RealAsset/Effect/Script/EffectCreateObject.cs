using UnityEngine;

public interface IEffectGameObject
{
    public void Effect();
    public void Return();
}

public abstract class EffectCreateObject<T> : Effect where T : IEffectGameObject
{
    [SerializeField] private int id;
    [SerializeField] private GameObject prefab;
    private static GameObject instance;

    public override void _Effect()
    {
        if (instance == null)
            instance = Instantiate(prefab, Wakgood.Instance.transform.position, Quaternion.identity);
        else
            instance.GetComponent<T>().Effect();
    }

    public override void Return()
    {
        if (DataManager.Instance.WakgoodItemInventory.itemCountDic[id] == 1)
            Destroy(instance.gameObject);
        else
            instance.GetComponent<T>().Return();
    }
}