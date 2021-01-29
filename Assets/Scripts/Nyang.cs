using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nyang : Loot
{
    public override void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ObjectManager.Instance.InsertQueue(PoolType.Nyang, gameObject);
            GameManager.Instance.AcquireNyang(Random.Range(10, 51)); 
        }
    }
}
