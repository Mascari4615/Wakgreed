using UnityEngine;

public abstract class PoolingObject : MonoBehaviour
{
    public PoolType poolType;

    public void InsertQueue()
    {
        if (poolType == PoolType.Slime1 || poolType == PoolType.Slime2 || poolType == PoolType.BossMonster)Debug.Log($"{name} : InsertQueue");
        ObjectManager.Instance.InsertQueue(poolType, gameObject);
    }
}
