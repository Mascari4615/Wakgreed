using UnityEngine;

public abstract class PoolableObject : MonoBehaviour
{
    protected ObjectPool<PoolableObject> pPool;

    public virtual void Create(ObjectPool<PoolableObject> pool)
    {
        pPool = pool;

        gameObject.SetActive(false);
    }

    public virtual void Dispose()
    {
        pPool.PushObject(this);
    }
}
