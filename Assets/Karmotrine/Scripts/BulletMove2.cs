using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class BulletMove2 : MonoBehaviour
{
    [SerializeField] private Vector3 directionVector;
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private Direction direction;
    [SerializeField] private Rigidbody2D Rigidbody2D;

    private void FixedUpdate()
    {
        Rigidbody2D.velocity = directionVector * moveSpeed * Time.deltaTime;
    }

    private void OnEnable()
    {
        switch (direction)
        {
            case Direction.Custom:
                return;
            case Direction.Traveller2Monster:
                directionVector = (Wakgood.Instance.AttackPosition.position -
                                   (Wakgood.Instance.transform.position + new Vector3(0, 0.8f, 0))).normalized;
                break;
            case Direction.Monster2Traveller:
                directionVector = (Wakgood.Instance.transform.position - transform.position).normalized;
                break;
            case Direction.Random:
                directionVector = new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), 0).normalized;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void SetDirection(Vector3 direction)
    {
        directionVector = direction.normalized;
    }

    private enum Direction
    {
        Custom,
        Monster2Traveller,
        Traveller2Monster,
        Random
    }
}