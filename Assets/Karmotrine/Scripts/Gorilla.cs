using System.Collections;
using UnityEngine;

public class Gorilla : NormalMonster
{
    [SerializeField] private GameObject damagingObject;
    [SerializeField] private GameObject earthQuake;
    [SerializeField] private GameObject earthQuakeWarning;
    private Coroutine checkWakgood;
    private Coroutine idle;
    private Coroutine rush;

    protected override void OnEnable()
    {
        base.OnEnable();

        Animator.SetTrigger("AWAKE");
        Animator.SetBool("ISMOVING", false);
        Animator.SetBool("ISRUSHING", false);

        damagingObject.SetActive(false);
        earthQuake.SetActive(false);
        earthQuakeWarning.SetActive(false);

        idle = StartCoroutine(Idle());
        checkWakgood = StartCoroutine(CheckWakgood());
    }

    protected override void OnDisable()
    {
        if (idle != null)
        {
            StopCoroutine(idle);
        }

        if (rush != null)
        {
            StopCoroutine(rush);
        }

        if (checkWakgood != null)
        {
            StopCoroutine(checkWakgood);
        }

        base.OnDisable();
    }

    private IEnumerator Idle()
    {
        while (true)
        {
            Vector2 direction = Random.insideUnitCircle.normalized;

            SpriteRenderer.flipX = direction.x > 0;

            Animator.SetBool("ISMOVING", true);
            for (int i = 0; i < 10; i++)
            {
                Rigidbody2D.velocity = direction * MoveSpeed * 0.3f;
                yield return ws01;
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
            if (Vector2.Distance(transform.position, Wakgood.Instance.transform.position) < 20)
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
        Rigidbody2D.velocity = Vector2.zero;

        yield return new WaitForSeconds(1.7f);

        while (true)
        {
            if (Vector2.Distance(transform.position, Wakgood.Instance.transform.position) > 12)
            {
                Animator.SetBool("ISMOVING", true);
                SpriteRenderer.flipX = IsWakgoodRight();
                Rigidbody2D.velocity = GetDirection() * MoveSpeed;
                yield return ws01;
            }
            else
            {
                Rigidbody2D.velocity = Vector2.zero;

                Animator.SetBool("ISMOVING", false);
                SpriteRenderer.flipX = IsWakgoodRight();

                yield return StartCoroutine(Casting(castingTime));
                Animator.SetBool("ISRUSHING", true);
                Vector2 direction = GetDirection();
                SpriteRenderer.flipX = IsWakgoodRight();
                damagingObject.SetActive(true);

                Rigidbody2D.velocity = Vector2.zero;
                for (float temptime = 0; temptime <= .8f; temptime += Time.fixedDeltaTime)
                {
                    if (Physics2D.BoxCast(transform.position, new Vector2(.7f, .7f), 0f, direction, 0.9f,
                            LayerMask.GetMask("Wall")).collider != null)
                    {
                        break;
                    }

                    Rigidbody2D.velocity = direction * 25;
                    yield return new WaitForFixedUpdate();
                }

                damagingObject.SetActive(false);
                Rigidbody2D.velocity = Vector2.zero;
                Animator.SetBool("ISRUSHING", false);

                yield return new WaitForSeconds(.2f);

                Animator.SetTrigger("ATTACKREADY");
                earthQuakeWarning.SetActive(true);
                yield return StartCoroutine(Casting(castingTime));

                GameManager.Instance.CinemachineImpulseSource.GenerateImpulse();
                Animator.SetTrigger("ATTACKGO");
                earthQuakeWarning.SetActive(false);
                earthQuake.SetActive(true);

                yield return new WaitForSeconds(1.7f);
            }
        }
    }
}