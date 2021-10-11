using UnityEngine;

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
        directionVector = direction;
    }

    private void OnEnable()
    {
        if (direction is not Direction.Custom)
        {
            if (direction.Equals(Direction.Traveller2Monster))
            {
                directionVector = (Wakgood.Instance.attackPosition.position - (Wakgood.Instance.transform.position + new Vector3(0, 0.8f, 0))).normalized;
            }
            else if (direction.Equals(Direction.Monster2Traveller))
            {
                directionVector = (Wakgood.Instance.transform.position - transform.position).normalized;
            }
            else if (direction.Equals(Direction.Random))
            {
                directionVector = new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), 0).normalized;
            }
        }
    }

    private void FixedUpdate()
    {
        transform.position += directionVector * moveSpeed * Time.deltaTime;
    }
}
