using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectiveObject : MonoBehaviour
{
    private Vector3 direction = Vector3.zero;
    public float speed = 1;

    void Awake()
    {
        SetDirection();
    }
    void Update()
    {
        transform.position += direction * Time.deltaTime * speed;
    }
    public void SetDirection()
    {
        direction = (GameManager.Instance.player.transform.position - transform.position).normalized;
    }
    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            gameObject.SetActive(false);
        }   
    }
}
