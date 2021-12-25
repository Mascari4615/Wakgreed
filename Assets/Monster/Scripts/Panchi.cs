using System.Collections;
using UnityEngine;

public class Panchi : NormalMonster
{
    private IEnumerator idle;
    private static readonly int ismoving = Animator.StringToHash("ISMOVING");
    private static readonly int attack = Animator.StringToHash("ATTACK");

    protected override void OnEnable()
    {
        base.OnEnable();

        idle = Idle();
        StartCoroutine(idle);
        StartCoroutine(CheckWakgood());
    }

    private IEnumerator Idle()
    {
        Vector2 spawnPos = transform.position;

        while (true)
        {
            Animator.SetBool(ismoving, true);
            Vector2 targetPos = spawnPos + Random.insideUnitCircle * 2;
            Vector2 originPos = transform.position;
            SpriteRenderer.flipX = (targetPos.x > originPos.x) ? true : false;
            for (int i = 0; i < 50; i++)
            {
                Rigidbody2D.position += (targetPos - originPos) * 0.02f;
                yield return new WaitForSeconds(0.02f);
            }
            Animator.SetBool(ismoving, false);
            yield return new WaitForSeconds(2f);
        }
    }

    private IEnumerator CheckWakgood()
    {
        WaitForSeconds ws01 = new(0.1f);
        while (true)
        {
            if (Vector2.Distance(transform.position, Wakgood.Instance.transform.position) < 7)
            {
                StopCoroutine(idle);
                StartCoroutine(Attack());
                break;
            }
            yield return ws01;
        }
    }

    private IEnumerator Attack()
    {
        WaitForSeconds ws002 = new(0.02f);
        Animator.SetBool(ismoving, false);

        while (true)
        {
            if (Vector2.Distance(transform.position, Wakgood.Instance.transform.position) > 2)
            {
                SpriteRenderer.flipX = Rigidbody2D.velocity.x > 0;
                Animator.SetBool(ismoving, true);
                Rigidbody2D.velocity = ((Vector2)Wakgood.Instance.transform.position - Rigidbody2D.position).normalized * MoveSpeed;
                yield return ws002;
            }
            else
            {
                Animator.SetBool(ismoving, false);
                Rigidbody2D.velocity = Vector2.zero;
                Vector3 direction = (Wakgood.Instance.transform.position - transform.position).normalized;
                Vector3 rot = new(0, 0, Mathf.Atan2(Wakgood.Instance.transform.position.y - (transform.position.y + 0.8f), Wakgood.Instance.transform.position.x - transform.position.x) * Mathf.Rad2Deg - 90);
                yield return StartCoroutine(Casting(.7f));
                Animator.SetTrigger(attack);
                ObjectManager.Instance.PopObject("PanchiSlash", transform.position + Vector3.up * 0.8f + direction * 1.5f, rot);
                yield return new WaitForSeconds(1f);
            }
        }
    }
}