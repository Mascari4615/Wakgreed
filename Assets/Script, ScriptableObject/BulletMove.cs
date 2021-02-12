using UnityEngine;

public class BulletMove : MonoBehaviour
{
    private enum Target
    {
        Monster,
        Traveller,
        Random
    }
    private Vector3 direction;
    [SerializeField] private float speed = 1f;
    [SerializeField] private Target target;

    void OnEnable()
    {
        if (target == Target.Monster) direction = (TravellerController.Instance.attackPosition.transform.position - TravellerController.Instance.transform.position).normalized;
        else if (target == Target.Traveller) direction = (Vector3)((Vector2)TravellerController.Instance.transform.position - (Vector2)transform.position).normalized;
        else if (target == Target.Random) direction = ((transform.position + new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), 0)) - transform.position).normalized;
    }

    void Update()
    {
        transform.position += direction * speed * Time.deltaTime;
    }
}
