using UnityEngine;

public class BulletRotate : MonoBehaviour
{
    private enum Direction
    {
        Clockwise,
        CounterClockwise,
        Random
    }
    private Vector3 rotationVector;
    [SerializeField] private float rotateSpeed = 50f;
    [SerializeField] private Direction direction = Direction.Clockwise;

    private void OnEnable()
    {
        if (direction == Direction.Clockwise)
        {
            // Means Rotate This Object Clockwise
            rotationVector = Vector3.forward;
        }
        else if (direction == Direction.CounterClockwise)
        {
            // Means Rotate This Object CounterClockwise
            rotationVector = Vector3.back;
        }
        else if (direction == Direction.Random)
        {
            rotationVector = Random.Range(0, 2) == 0 ? Vector3.forward : Vector3.back;
        }

        if (rotateSpeed <= 0)
        {
            rotateSpeed = Random.Range(-100f, 100f);
        }
    }

    private void FixedUpdate()
    {
        transform.Rotate(rotationVector * rotateSpeed * Time.deltaTime);
    }
}
