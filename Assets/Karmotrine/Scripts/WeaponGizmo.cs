using UnityEngine;

public class WeaponGizmo : MonoBehaviour
{
    private const int Step = 10;

    [SerializeField] [Range(0, 10)] [Tooltip("공격 범위")]
    private float radius = 1;

    [SerializeField] [Range(0, 10)] [Tooltip("공격 콜라이더 생성 위치")]
    private float distance = 1.5f;

    private Vector3 attackPosition;

    private Transform attackTransform;
    private Vector3 cross, prev, next;
    private float theta;

    private void Awake()
    {
        if (Wakgood.Instance is not null)
        {
            attackTransform = Wakgood.Instance.AttackPosition;
        }
        else
        {
            attackTransform = transform;
        }
    }

    private void OnDrawGizmos()
    {
        theta = 360f / Step;
        cross = Vector3.Cross(Vector3.forward, Vector3.up);
        if (cross.magnitude == 0f)
        {
            cross = Vector3.forward;
        }

        Gizmos.color = Color.red;

        if (attackTransform == null)
        {
            attackPosition = transform.position + (Vector3.right * distance);
        }
        else
        {
            attackPosition = attackTransform.position;
        }

        for (int i = 1; i <= Step; ++i)
        {
            prev = attackPosition + (Quaternion.AngleAxis(theta * (i - 1), Vector3.forward) * cross * radius);
            next = attackPosition + (Quaternion.AngleAxis(theta * i, Vector3.forward) * cross * radius);
            Gizmos.DrawLine(prev, next);
        }
    }
}