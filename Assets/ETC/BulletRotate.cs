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
    private float curRotateSpeed = 50f;
    [SerializeField] private Direction direction = Direction.Clockwise;
    [SerializeField] private float weight = 0;

    private void OnEnable()
    {
        curRotateSpeed = rotateSpeed;
        rotationVector = direction switch
        {
            Direction.Clockwise => Vector3.forward,
            Direction.CounterClockwise => Vector3.back,
            Direction.Random => Random.Range(0, 2) == 0 ? Vector3.forward : Vector3.back,
            _ => rotationVector
        };

        if (curRotateSpeed <= 0)
        {
            curRotateSpeed = Random.Range(-100f, 100f);
        }
    }

    private void FixedUpdate()
    {
        transform.Rotate(rotationVector * (curRotateSpeed += weight) * Time.deltaTime);
    }
}
