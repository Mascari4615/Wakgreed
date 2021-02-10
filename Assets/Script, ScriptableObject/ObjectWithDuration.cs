using UnityEngine;

public class ObjectWithDuration : PoolingObject
{
    private float removeTime = 0f;
    [SerializeField] private float duration = 0f;

    private void OnEnable()
    {
        removeTime = Time.time + duration;
    }

    private void Update()
    {
        if (removeTime <= Time.time)
        {
            InsertQueue();
        }      
    }
}
