using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BSlash : MonoBehaviour
{
    private Vector3 directionVector;
    private float moveSpeed = 1f;
    private float temp = 1f;

    private void OnEnable()
    {
        temp = 0;
        moveSpeed = 0;
    }

    public void SetDirection(Vector3 direction)
    {
        directionVector = direction;
    }

    private void FixedUpdate()
    {
        temp += Time.deltaTime;
        transform.position += directionVector * ((moveSpeed += temp * 1.5f) * Time.deltaTime);
    }
}
