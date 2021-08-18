using UnityEngine;

public class PoolingObject : MonoBehaviour
{
    // [SerializeField] private PoolType poolType;

    private void OnDisable()
    {
        // Debug.Log($"{name} : InsertQueue");
        //ObjectManager.Instance.InsertQueue(poolType, gameObject);
        ObjectManager.Instance.InsertQueue(gameObject);
    }
}
