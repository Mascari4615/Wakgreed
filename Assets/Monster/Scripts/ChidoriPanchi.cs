using System.Collections;
using UnityEngine;

public class ChidoriPanchi : NormalMonster
{
    private Coroutine idle;
    private Coroutine checkWakgood;
    private Coroutine attack;

    protected override void OnEnable()
    {
        base.OnEnable();

        Animator.SetBool("ISMOVING", false);
        idle = StartCoroutine(Idle());
        checkWakgood = StartCoroutine(CheckWakgood());
    }

    private IEnumerator Idle()
    {
        Vector2 spawnPos = transform.position;
        yield return new WaitForSeconds(1.5f);

        while (true)
        {
            Animator.SetBool("ISMOVING", true);
            Vector2 targetPos = spawnPos + Random.insideUnitCircle * 2;
            Vector2 originPos = transform.position;
            SpriteRenderer.flipX = targetPos.x > originPos.x;
            for (int i = 0; i < 50; i++)
            {
                Rigidbody2D.position += (targetPos - originPos) * 0.02f;
                yield return new WaitForSeconds(0.02f);
            }
            Animator.SetBool("ISMOVING", false);
            yield return new WaitForSeconds(2f);
        }
    }

    private IEnumerator CheckWakgood()
    {
        WaitForSeconds ws01 = new(0.1f);
        while (true)
        {
            if (Vector2.Distance(transform.position, Wakgood.Instance.transform.position) < 30)
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
        WaitForSeconds ws002 = new(0.02f);
        while (true)
        {
            if (Vector2.Distance(transform.position, Wakgood.Instance.transform.position) > 2)
            {
                SpriteRenderer.flipX = Rigidbody2D.velocity.x > 0;
                Animator.SetBool("ISMOVING", true);
                Rigidbody2D.velocity = ((Vector2)Wakgood.Instance.transform.position - Rigidbody2D.position).normalized * MoveSpeed;
                yield return ws002;
            }
            else
            {
                Animator.SetBool("ISMOVING", false);
                Rigidbody2D.velocity = Vector2.zero;
                Vector3 direction = (Wakgood.Instance.transform.position - transform.position).normalized;
                Vector3 rot = new(0, 0, Mathf.Atan2(Wakgood.Instance.transform.position.y - (transform.position.y + 0.8f), Wakgood.Instance.transform.position.x - transform.position.x) * Mathf.Rad2Deg - 90);
                yield return StartCoroutine(Casting(.3f));
                Animator.SetTrigger("ATTACK");
                ObjectManager.Instance.PopObject("Chidori", transform.position + Vector3.up * 0.8f + direction * 1.5f, rot);
                yield return new WaitForSeconds(1f);
            }
        }
    }

    protected override void OnDisable()
    {
        if (idle != null) StopCoroutine(idle);
        if (checkWakgood != null) StopCoroutine(checkWakgood);
        if (attack != null) StopCoroutine(attack);
        base.OnDisable();
    }
}
