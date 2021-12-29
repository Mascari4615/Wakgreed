using System.Collections;
using UnityEngine;

public class Gorilla : NormalMonster
{
    private IEnumerator idle;
    [SerializeField] private GameObject damagingObject;
    [SerializeField] private GameObject earthQuake;
    [SerializeField] private GameObject earthQuakeWarning;

    protected override void OnEnable()
    {
        base.OnEnable();

        damagingObject.SetActive(false);
        earthQuake.SetActive(false);
        earthQuakeWarning.SetActive(false);

        idle = Idle();
        StartCoroutine(idle);
        StartCoroutine(CheckWakgood());
    }

    private IEnumerator Idle()
    {
        Vector2 spawnPos = transform.position;

        while (true)
        {
            Animator.SetBool("ISMOVING", true);
            Vector2 targetPos = spawnPos + Random.insideUnitCircle * 2;
            Vector2 originPos = transform.position;
            SpriteRenderer.flipX = targetPos.x > originPos.x;

            for (float i = 0; i <= 1; i += Time.deltaTime)
            {
                Rigidbody2D.position = Vector2.Lerp(originPos, targetPos, i);
                yield return null;
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
                StartCoroutine(Rush());
                break;
            }
            yield return ws01;
        }
    }

    private IEnumerator Rush()
    {
        WaitForSeconds ws002 = new(0.02f);

        Animator.SetBool("ISMOVING", false);
        yield return new WaitForSeconds(2f);

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
                Rigidbody2D.velocity = Vector2.zero;

                Animator.SetBool("ISMOVING", false);
                Vector2 direction = (Wakgood.Instance.transform.position - transform.position).normalized;
                SpriteRenderer.flipX = direction.x > 0;

                yield return StartCoroutine(Casting(.7f));
                Animator.SetBool("ISMOVING", true);
                damagingObject.SetActive(true);

                Rigidbody2D.velocity = Vector2.zero;
                for (float temptime = 0; temptime <= 1f; temptime += Time.fixedDeltaTime)
                {
                    if (Physics2D.BoxCast(transform.position, new Vector2(.5f, .5f), 0f, direction, 0.9f, LayerMask.GetMask("Wall")).collider != null) break;

                    Rigidbody2D.velocity = direction * 20;
                    yield return new WaitForFixedUpdate();
                }
                Rigidbody2D.velocity = Vector2.zero;
                Animator.SetBool("ISMOVING", false);
                damagingObject.SetActive(false);

                yield return new WaitForSeconds(.5f);

                Animator.SetTrigger("ATTACKREADY");
                earthQuakeWarning.SetActive(true);
                yield return StartCoroutine(Casting(.5f));

                Animator.SetTrigger("ATTACKGO");
                earthQuakeWarning.SetActive(false);
                earthQuake.SetActive(true);
                GameManager.Instance.CinemachineImpulseSource.GenerateImpulse(5);

                yield return new WaitForSeconds(5f);
            }
        }
    }
}
