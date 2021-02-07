using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpOrb : Loot
{
    public override void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ObjectManager.Instance.InsertQueue(PoolType.Exp, gameObject);
            TravellerController.Instance.traveller.AcquireExp(Random.Range(10, 21));
        }      
    }
}
