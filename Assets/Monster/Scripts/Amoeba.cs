using System.Collections;
using UnityEngine;

public class Amoeba : NormalMonster
{
    private static readonly WaitForSeconds ws2 = new (2);

    protected override void OnEnable()
    {
        base.OnEnable();
        StartCoroutine(Idle());
    }

    private IEnumerator Idle()
    {
        while (true)
        {
            float t = Random.Range(1f, 3f);
            Vector2 direction = (Vector3)Random.insideUnitCircle.normalized;

            Animator.SetBool("MOVING", true);
            SpriteRenderer.flipX = direction.x < 0;

            while (t > 0)
            {
                Rigidbody2D.velocity = direction * MoveSpeed;
                t -= 0.1f;
                yield return ws01;
            }

            Animator.SetBool("MOVING", false);
            yield return ws2;
        }
    }

}
