using System.Collections;
using UnityEngine;

public class Gorilla : NormalMonster
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
            Animator.SetBool("ISMOVING", true);
            Vector2 targetPos = spawnPos + new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 2f), 0);
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
                SpriteRenderer.flipX = Rigidbody2D.velocity.x > 0;
                Animator.SetBool("ISMOVING", true);
                Rigidbody2D.velocity = ((Vector2)Wakgood.Instance.transform.position - Rigidbody2D.position).normalized * MoveSpeed;
                yield return ws002;
            }
            else
            {
                Animator.SetBool("ISMOVING", false);
                Vector2 direction = (Wakgood.Instance.transform.position - transform.position).normalized;
                yield return new WaitForSeconds(1f);
                Animator.SetBool("ISMOVING", true);
                damagingObject.SetActive(true);

                Rigidbody2D.velocity = Vector2.zero;
                for (float temptime = 0; temptime <= 2f; temptime += Time.deltaTime)
                {
                    if (Physics2D.BoxCast(transform.position, new Vector2(.5f, .5f), 0f, direction, 0.9f, LayerMask.GetMask("Wall")).collider != null) break;

                    Rigidbody2D.velocity = direction * 10;
                    yield return new WaitForFixedUpdate();
                }
                Rigidbody2D.velocity = Vector2.zero;
                Animator.SetBool("ISMOVING", false);
                damagingObject.SetActive(false);
                yield return new WaitForSeconds(5f);
            }
        }
    }
}
