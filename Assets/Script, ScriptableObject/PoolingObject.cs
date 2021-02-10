using UnityEngine;

public abstract class PoolingObject : MonoBehaviour
{
    public PoolType poolType;

    public void InsertQueue()
    {
        ObjectManager.Instance.InsertQueue(poolType, gameObject);
    }
}
