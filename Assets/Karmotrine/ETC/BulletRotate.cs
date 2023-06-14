using UnityEngine;

public class BulletRotate : MonoBehaviour
{
    public float rotateSpeed;
    [SerializeField] private Direction direction;
    private Vector3 rotationVector;

    private void Awake()
    {
        rotationVector = direction switch
        {
            Direction.Clockwise => Vector3.forward,
            Direction.CounterClockwise => Vector3.back,
            Direction.Random => Random.Range(0, 2) == 0 ? Vector3.forward : Vector3.back,
            _ => rotationVector
        };
    }

    private void FixedUpdate()
    {
        transform.Rotate(rotationVector * rotateSpeed * Time.deltaTime);
    }

    private enum Direction
    {
        Clockwise,
        CounterClockwise,
        Random
    }
}