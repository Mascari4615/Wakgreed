using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DraggedObject : MonoBehaviour
{
    float t1, t2 = 0;
    public Vector3 waitPosition = Vector3.zero;
    [SerializeField] float waitTime = 0.5f;
    [SerializeField] float moveSpeed = 1;
    [SerializeField] CircleCollider2D circleCollider2D = null;
    [SerializeField] AudioSource audioSource = null;
    [SerializeField] SpriteRenderer spriteRenderer = null;
    [SerializeField] PoolType poolType = PoolType.Nothing;

    void OnEnable()
    {
        circleCollider2D.enabled = false;
        t1 = 0;
        t2 = 0;
    }

    void Update()
    {
        if (t1 < waitTime)
        {        
            t1 += Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, waitPosition, t1 * 0.6f); 
        }
        else
        {
            circleCollider2D.enabled = true;
            t2 += Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, GameManager.Instance.player.transform.position, t2 * moveSpeed); 
        }      
    }
}
