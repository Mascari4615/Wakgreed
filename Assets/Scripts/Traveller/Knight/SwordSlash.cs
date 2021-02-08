using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordSlash : MonoBehaviour
{
    [SerializeField] private IntVariable AD;
    private float t1 = 0f;
    [SerializeField] private float t2 = 0f;
    [SerializeField] private PoolType poolType = PoolType.Nothing;

    void Update()
    {
        t1 += Time.deltaTime;
        if (t1 >= t2)
        {
            ObjectManager.Instance.InsertQueue(poolType, gameObject);
            t1 = 0;
        }      
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Monster")
        {
            if (other.gameObject.TryGetComponent<Monster>(out Monster monster))
            {
                monster.ReceiveDamage(AD.RuntimeValue);
            }
        }
    }
}
