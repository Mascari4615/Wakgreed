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

    public void SetDirection(Vector3 direction)
    {
        directionVector = direction.normalized;
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
    }

    private void FixedUpdate()
    {
        transform.position += directionVector * moveSpeed * Time.deltaTime;
    }
}
