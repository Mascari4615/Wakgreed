using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loot : MonoBehaviour
{
    [HideInInspector] public Vector3 waitPosition = Vector3.zero;
    protected float waitTime = 0.5f;
    protected float currentWaitTime = 0.5f;
    protected float waitMoveSpeed = 0;
    protected float moveSpeed = 0;
    protected CircleCollider2D circleCollider2D;
    protected AudioSource audioSource;
    protected SpriteRenderer spriteRenderer;

    protected void Awake()
    {
        circleCollider2D = GetComponent<CircleCollider2D>();
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public virtual void OnEnable()
    {
        circleCollider2D.enabled = false;
        currentWaitTime = waitTime;
        waitMoveSpeed = 0;
        moveSpeed = 0;
    }

    protected void Update()
    {
        if (currentWaitTime > 0)
        {
            waitMoveSpeed += Time.deltaTime;
            currentWaitTime -= Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, waitPosition, waitMoveSpeed * 0.6f); 
        }
        else
        {
            moveSpeed += Time.deltaTime;
            circleCollider2D.enabled = true;
            transform.position = Vector3.Lerp(transform.position, TravellerController.Instance.transform.position, moveSpeed); 
        }      
    }

    public virtual void OnTriggerStay2D(Collider2D other)
    {

    }
}
