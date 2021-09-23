using UnityEngine;

public class BulletMove : MonoBehaviour
{
    private enum Direction
    {
        Monster2Traveller,
        Traveller2Monster,
        Random
    }
    private Vector3 directionVector;
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private Direction direction;

    private void OnEnable()
    {
        if (direction == Direction.Traveller2Monster)
        {
            // Means Spawn At Travellers AttackPosition
            directionVector = (Wakgood.Instance.attackPosition.position - (Wakgood.Instance.transform.position + new Vector3(0, 0.8f, 0))).normalized;
        }
        else if (direction == Direction.Monster2Traveller)
        {
            // Means Spawn At Monsters AttackPosition
            directionVector = (Wakgood.Instance.transform.position - transform.position).normalized;
        }
        else if (direction == Direction.Random)
        {
            directionVector = new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), 0).normalized;
        }
    }

    private void FixedUpdate()
    {
        transform.position += directionVector * moveSpeed * Time.deltaTime;
    }
}
