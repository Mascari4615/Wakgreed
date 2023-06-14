using UnityEngine;

public interface IEffectGameObject
{
    public void Effect();
    public void Return();
}

public abstract class EffectCreateObject<T> : Effect where T : IEffectGameObject
{
    private static GameObject instance;
    [SerializeField] private int id;
    [SerializeField] private GameObject prefab;
    [SerializeField] private bool onWakgood;

    public override void _Effect()
    {
        if (instance == null)
        {
            instance = onWakgood
                ? Instantiate(prefab, Wakgood.Instance.transform)
                : Instantiate(prefab, Wakgood.Instance.transform.position, Quaternion.identity);
        }
        else
        {
            instance.GetComponent<T>().Effect();
        }
    }

    public override void Return()
    {
        if (DataManager.Instance.wgItemInven.itemCountDic[id] == 1)
        {
            Destroy(instance);
        }
        else
        {
            instance.GetComponent<T>().Return();
        }
    }
}