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
            animator.SetBool("IsMoving", true);
            Vector2 targetPos = spawnPos + new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 2f), 0);
            Vector2 originPos = transform.position;
            for (int i = 0; i < 50; i++)
            {
                rigidbody2D.position += (targetPos - originPos) * 0.02f;
                yield return new WaitForSeconds(0.02f);
            }
            animator.SetBool("IsMoving", false);
            yield return new WaitForSeconds(2f);
        }
    }

    private IEnumerator CheckWakgood()
    {
        WaitForSeconds ws01 = new(0.1f);
        while (true)
        {
            if (Vector2.Distance(transform.position, TravellerController.Instance.transform.position) < 10)
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
            if (Vector2.Distance(transform.position, TravellerController.Instance.transform.position) > 3)
            {
                // 길찾기
                // rigidbody2D.position += ((Vector2)TravellerController.Instance.transform.position - rigidbody2D.position).normalized * Time.deltaTime * moveSpeed;
                rigidbody2D.velocity = ((Vector2)TravellerController.Instance.transform.position - rigidbody2D.position).normalized * moveSpeed;
                yield return ws002;
            }
            else
            {
                animator.SetTrigger("ATTACK");
                yield return new WaitForSeconds(3f);
            }
        }
    }
}
