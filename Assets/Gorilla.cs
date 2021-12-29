using System.Collections;
using UnityEngine;

public class Gorilla : NormalMonster
{
    private Coroutine idle;
    private Coroutine rush;
    [SerializeField] private GameObject damagingObject;
    [SerializeField] private GameObject earthQuake;
    [SerializeField] private GameObject earthQuakeWarning;

    protected override void OnEnable()
    {
        base.OnEnable();

        Animator.SetTrigger("AWAKE");
        Animator.SetBool("ISMOVING", false);

        damagingObject.SetActive(false);
        earthQuake.SetActive(false);
        earthQuakeWarning.SetActive(false);

        idle = StartCoroutine(Idle());
        StartCoroutine(CheckWakgood());
    }

    private IEnumerator Idle()
    {
        while (true)
        {
            Vector2 direction = Random.insideUnitCircle.normalized;

            SpriteRenderer.flipX = direction.x > 0;

            Animator.SetBool("ISMOVING", true);
            for (float i = 0; i <= 1; i += Time.deltaTime)
            {
                Rigidbody2D.velocity = direction;
                yield return null;
            }
            Rigidbody2D.velocity = Vector2.zero;
            Animator.SetBool("ISMOVING", false);

            yield return new WaitForSeconds(Random.Range(3f, 6f));
        }
    }

    private IEnumerator CheckWakgood()
    {
        while (true)
        {
            if (Vector2.Distance(transform.position, Wakgood.Instance.transform.position) < 7)
            {
                StopCoroutine(idle);
                rush = StartCoroutine(Rush());
                break;
            }
            yield return ws01;
        }
    }

    private IEnumerator Rush()
    {
        Animator.SetBool("ISMOVING", false);
        yield return new WaitForSeconds(2f);

        while (true)
        {
            if (Vector2.Distance(transform.position, Wakgood.Instance.transform.position) > 10)
            {
                SpriteRenderer.flipX = Rigidbody2D.velocity.x > 0;
                Animator.SetBool("ISMOVING", true);
                Rigidbody2D.velocity = ((Vector2)Wakgood.Instance.transform.position - Rigidbody2D.position).normalized * MoveSpeed;
                yield return ws01;
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
                yield return StartCoroutine(Casting(.6f));

                Animator.SetTrigger("ATTACKGO");
                earthQuakeWarning.SetActive(false);
                earthQuake.SetActive(true);
                GameManager.Instance.CinemachineImpulseSource.GenerateImpulse(5);

                yield return new WaitForSeconds(5f);
            }
        }
    }

    private void OnDisable()
    {
        if (idle != null) StopCoroutine(idle);
        if (rush != null) StopCoroutine(rush);
    }
}
