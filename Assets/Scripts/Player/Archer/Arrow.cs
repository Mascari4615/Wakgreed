using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    private float t1 = 0f;
    [SerializeField] private float t2 = 0f;
    [SerializeField] private PoolType poolType = PoolType.Nothing;
    Vector3 direction = Vector3.zero;
    [SerializeField] private float speed = 1f;

    void OnEnable()
    {
        direction = (Traveller.Instance.attackPosition.transform.position - Traveller.Instance.transform.position).normalized;
    }

    void Update()
    {
        t1 += Time.deltaTime;
        if (t1 >= t2)
        {
            ObjectManager.Instance.InsertQueue(poolType, gameObject);
            t1 = 0;
        }
        transform.position += direction * speed;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Monster")
        {
            if (other.gameObject.TryGetComponent<Monster>(out Monster monster))
            {
                monster.ReceiveDamage(Traveller.Instance.ad);
            }
        }
    }
}
