using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeckPlane : MonoBehaviour
{
    [SerializeField]  private float speed;
  private float size;
    [SerializeField] private float maxSize = 5f;
    [SerializeField] private float minSize = 1f;

    private void OnEnable()
    {
        speed *= Random.Range(1f, 5f);
        size = Random.Range(minSize, maxSize);
        transform.localScale = Vector3.one * size;

    }

    private void Update()
    {
        transform.position += new Vector3(0, 1, 0) * Time.deltaTime * speed;
        if (transform.position.y >= 30)
            Destroy(gameObject);
    }
}
