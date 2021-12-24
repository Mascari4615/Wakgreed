using UnityEngine;

public class BulletMoveStar : MonoBehaviour
{
    float t = 0;
    [SerializeField] private float speed = 1;
    [SerializeField] private float n = 1;
    private Vector3 origin;

    private void OnEnable()
    {
        origin = transform.position;
    }

    public void Set(float _speed, float _n)
    {
        speed = _speed;
        n = _n;
    }

    private void FixedUpdate()
    {
        t += Time.fixedDeltaTime * speed;
        transform.position = origin + new Vector3(5 * Mathf.Cos(2 * t) + 2 * Mathf.Cos(3 * t), 2 * Mathf.Sin(3 * t) - 5 * Mathf.Sin(2 * t), 0) * n;
    }
}
