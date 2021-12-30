using System.Collections;
using UnityEngine;

public class Panchi : NormalMonster
{
    private Coroutine idle;
    private Coroutine attack;
    private Coroutine checkDistance;

    protected override void OnEnable()
    {
        base.OnEnable();

        idle = StartCoroutine(Idle());
        checkDistance = StartCoroutine(CheckWakgood());
    }

    private IEnumerator Idle()
    {
        while (true)
        {
            Vector2 direction = Random.insideUnitCircle * 2;
            SpriteRenderer.flipX = direction.x > 0;
            Animator.SetBool("ISMOVING", true);
            for (float i = 0; i <= 1; i += Time.fixedDeltaTime)
            {
                Rigidbody2D.velocity = direction * MoveSpeed;
                yield return new WaitForFixedUpdate();
            }
            Animator.SetBool("ISMOVING", false);
            yield return ws1;
            yield return ws1;
        }
    }

    private IEnumerator CheckWakgood()
    {
        while (true)
        {
            if (Vector2.Distance(transform.position, Wakgood.Instance.transform.position) < 7)
            {
                StopCoroutine(idle);
                attack = StartCoroutine(Attack());
                break;
            }
            yield return ws01;
        }
    }

    private IEnumerator Attack()
    {
        while (true)
        {
            if (Vector2.Distance(transform.position, Wakgood.Instance.transform.position) > 2)
            {
                Animator.SetBool("ISMOVING", true);
                SpriteRenderer.flipX = Rigidbody2D.velocity.x > 0;
                Rigidbody2D.velocity = ((Vector2)Wakgood.Instance.transform.position - Rigidbody2D.position).normalized * MoveSpeed;
                yield return ws01;
            }
            else
            {
                Animator.SetBool("ISMOVING", false);
                Rigidbody2D.velocity = Vector2.zero;
                yield return StartCoroutine(Casting(.7f));
                Animator.SetTrigger("ATTACK");
                ObjectManager.Instance.PopObject("PanchiSlash", transform.position + Vector3.up * 0.8f + GetDirection() * 1.5f, GetRot());
                yield return ws1;
            }
        }
    }

    private void OnDisable()
    {
        if (idle != null) StopCoroutine(idle);
        if (checkDistance != null) StopCoroutine(checkDistance);
        if (attack != null) StopCoroutine(attack);
    }
}