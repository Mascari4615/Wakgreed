using System.Collections;
using UnityEngine;

public class ChidoriPanchi : MeleePanchi
{
    private IEnumerator idle;

    protected override void OnEnable()
    {
        base.OnEnable();

        idle = Idle();
        StartCoroutine(idle);
        StartCoroutine(CheckWakgood());
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
        while (true)
        {
            if (Vector2.Distance(transform.position, Wakgood.Instance.transform.position) > 2)
            {
                spriteRenderer.flipX = (rigidbody2D.velocity.x > 0) ? true : false;
                animator.SetBool("ISMOVING", true);
                // 길찾기
                // rigidbody2D.position += ((Vector2)TravellerController.Instance.transform.position - rigidbody2D.position).normalized * Time.deltaTime * moveSpeed;
                rigidbody2D.velocity = ((Vector2)Wakgood.Instance.transform.position - rigidbody2D.position).normalized * moveSpeed;
                yield return ws002;
            }
            else
            {
                animator.SetBool("ISMOVING", false);
                rigidbody2D.velocity = Vector2.zero;
                Vector3 direction = (Wakgood.Instance.transform.position - transform.position).normalized;
                Vector3 rot = new Vector3(0, 0, Mathf.Atan2(Wakgood.Instance.transform.position.y - (transform.position.y + 0.8f), Wakgood.Instance.transform.position.x - transform.position.x) * Mathf.Rad2Deg - 90);
                yield return new WaitForSeconds(.7f);              
                animator.SetTrigger("ATTACK");
                ObjectManager.Instance.PopObject("MobSlash", transform.position + Vector3.up * 0.8f + direction * 1.5f, rot);
                yield return new WaitForSeconds(1f);
            }
        }
    }
}
