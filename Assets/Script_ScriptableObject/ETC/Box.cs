using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour, Damagable
{
    [SerializeField] private GameObject[] Fragments;

    public void ReceiveDamage(int damage)
    {
        Debug.Log("ASD");
        Break();
    }

    public void Break()
    {
        foreach (var fragment in Fragments)
        {
            fragment.SetActive(true);
            fragment.GetComponent<Rigidbody2D>().AddForce(new Vector2(Random.Range(-1f,1f), Random.Range(-1f,1f) * Random.Range(1,3f)));
        }
        GetComponent<SpriteRenderer>().enabled = false;
        this.enabled = false;
    }
}
