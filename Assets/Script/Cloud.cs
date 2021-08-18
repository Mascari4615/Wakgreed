using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : MonoBehaviour
{
    private float speed;
    private float size;

    private void OnEnable()
    {
        speed = Random.Range(1f, 5f);
        size = Random.Range(1f, 5f);

        transform.localScale = Vector3.one * size;
    }

    private void Update()
    {
        transform.position -= new Vector3(1, 0, 0) * Time.deltaTime * speed;
        //if (transform.position.x <= -60) ObjectManager.Instance.InsertQueue(PoolType.Cloud, gameObject);
        if (transform.position.x <= -60) ObjectManager.Instance.InsertQueue(gameObject);
    }
}
