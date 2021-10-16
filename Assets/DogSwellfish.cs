using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogSwellfish : MeleePanchi
{
    private IEnumerator idle;
    private IEnumerator attack;

    protected override void OnEnable()
    {
        base.OnEnable();

        idle = Idle();
        StartCoroutine(idle);
        attack = Attack();
    }

    private IEnumerator Idle()
    {
        Vector3 spawnPos = transform.position;

        while (true)
        {
            animator.SetBool("ISMOVING", true);
            Vector2 targetPos = spawnPos + new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 2f), 0);
            Vector2 originPos = transform.position;
            spriteRenderer.flipX = (targetPos.x > originPos.x) ? true : false;
            for (int i = 0; i < 50; i++)
            {
                rigidbody2D.position += (targetPos - originPos) * 0.02f;
                yield return new WaitForSeconds(0.02f);
            }
            animator.SetBool("ISMOVING", false);
            yield return new WaitForSeconds(2f);
        }
    }

    private IEnumerator Attack()
    {
        WaitForSeconds ws002 = new(0.02f);
        animator.SetTrigger("ATTACK");
        while (true)
        {
            rigidbody2D.velocity = Vector2.zero;
            Vector3 direction = (Wakgood.Instance.transform.position - transform.position).normalized;
            Vector3 rot = new(0, 0, Mathf.Atan2(Wakgood.Instance.transform.position.y - (transform.position.y + 0.8f), Wakgood.Instance.transform.position.x - transform.position.x) * Mathf.Rad2Deg - 90);
            yield return new WaitForSeconds(.7f);     
            ObjectManager.Instance.PopObject("PanchiSlash", transform.position + Vector3.up * 0.8f + direction * 1.5f, rot);
            yield return new WaitForSeconds(1f);
        }
    }

    public override void ReceiveHit(int damage)
    {
        if (isCollapsed) return;

        StopCoroutine(idle);
        StopCoroutine(attack);
        StartCoroutine(attack = Attack());

        base.ReceiveHit(damage);
    }
}
