using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class BulletMove : MonoBehaviour
{
    private enum Direction
    {
        Custom,
        Monster2Traveller,
        Traveller2Monster,
        Random
    }
    [SerializeField] private Vector3 directionVector;
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private Direction direction;
    private bool canMove = true;

    public void SetDirection(Vector3 direction)
    {
        directionVector = direction.normalized;
    }

    public void StopMove()
    {
        canMove = false;
    }

    public void Move()
    {
        canMove = true;
    }

    private void OnEnable()
    {
        switch (direction)
        {
            case Direction.Custom:
                return;
            case Direction.Traveller2Monster:
                directionVector = (Wakgood.Instance.AttackPosition.position - (Wakgood.Instance.transform.position + new Vector3(0, 0.8f, 0))).normalized;
                break;
            case Direction.Monster2Traveller:
                directionVector = (Wakgood.Instance.transform.position - transform.position).normalized;
                break;
            case Direction.Random:
                directionVector = Random.insideUnitCircle.normalized;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        canMove = true;
    }

    private void FixedUpdate()
    {
        if (canMove)
            transform.position += directionVector * moveSpeed * Time.deltaTime;
    }
}
