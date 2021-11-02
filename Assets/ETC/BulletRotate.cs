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
        rotationVector = direction switch
        {
            Direction.Clockwise => Vector3.forward,
            Direction.CounterClockwise => Vector3.back,
            Direction.Random => Random.Range(0, 2) == 0 ? Vector3.forward : Vector3.back,
            _ => rotationVector
        };

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
