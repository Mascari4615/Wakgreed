using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectiveObject : MonoBehaviour
{
    private Vector3 direction = Vector3.zero;
    public float speed = 1;

    void OnEnable()
    {
        SetDirection();
    }

    void Update()
    {
        transform.position += direction * Time.deltaTime * speed;
    }

    void SetDirection()
    {
        direction = (TravellerController.Instance.transform.position - transform.position).normalized;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            TravellerController.Instance.ReceiveDamage(5);
            gameObject.SetActive(false);
        }   
    }
}