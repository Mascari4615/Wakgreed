using UnityEngine;

public class BulletMove : MonoBehaviour
{
    private enum Target
    {
        Monster,
        Traveller
    }
    private Vector3 direction;
    [SerializeField] private float speed = 1f;
    [SerializeField] private Target target;

    void OnEnable()
    {
        direction = (target == Target.Monster) ?
        (TravellerController.Instance.attackPosition.transform.position - TravellerController.Instance.transform.position).normalized 
        : (Vector3)((Vector2)TravellerController.Instance.transform.position - (Vector2)transform.position).normalized;
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }
}
