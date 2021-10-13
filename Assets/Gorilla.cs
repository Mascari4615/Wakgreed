using System.Collections;
using UnityEngine;

public class Gorilla : MeleePanchi
{
    private IEnumerator idle;
    [SerializeField] private GameObject damagingObject;

    protected override void Awake()
    {
        base.Awake();
    }

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
            spriteRenderer.flipX = targetPos.x > originPos.x;
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
            if (Vector2.Distance(transform.position, Wakgood.Instance.transform.position) > 10)
            {
                spriteRenderer.flipX = rigidbody2D.velocity.x > 0;
                animator.SetBool("ISMOVING", true);
                rigidbody2D.velocity = ((Vector2)Wakgood.Instance.transform.position - rigidbody2D.position).normalized * moveSpeed;
                yield return ws002;
            }
            else
            {
                animator.SetBool("ISMOVING", false);
                Vector2 direction = (Wakgood.Instance.transform.position - transform.position).normalized;
                yield return new WaitForSeconds(1f);
                animator.SetBool("ISMOVING", true);
                damagingObject.SetActive(true);

                rigidbody2D.velocity = Vector2.zero;
                for (float temptime = 0; temptime <= 2f; temptime += Time.deltaTime)
                {
                    if (Physics2D.BoxCast(transform.position, new Vector2(.5f, .5f), 0f, direction, 0.9f, LayerMask.GetMask("Wall")).collider != null) break;

                    rigidbody2D.velocity = direction * 10;
                    yield return new WaitForFixedUpdate();
                }
                rigidbody2D.velocity = Vector2.zero;
                animator.SetBool("ISMOVING", false);
                damagingObject.SetActive(false);
                yield return new WaitForSeconds(5f);
            }
        }
    }
}
