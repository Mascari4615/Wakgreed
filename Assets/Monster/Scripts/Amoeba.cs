using System.Collections;
using UnityEngine;

public class Amoeba : NormalMonster
{
    private IEnumerator idle;
    private IEnumerator attack;
    private IEnumerator ahya;
    private IEnumerator checkWakgood;
    private bool bRecognizeWakgood = false;
    [SerializeField] private Collider2D bodyCollider;
    private static readonly int ismoving = Animator.StringToHash("ISMOVING");

    protected override void OnEnable()
    {
        base.OnEnable();

        bodyCollider.enabled = true;

        attack = Attack();
        ahya = Ahya();
        StartCoroutine(idle = Idle());
        StartCoroutine(checkWakgood = CheckWakgood());
    }

    private IEnumerator CheckWakgood()
    {
        WaitForSeconds ws01 = new(0.1f);
        while (true)
        {
            if (Vector2.Distance(transform.position, Wakgood.Instance.transform.position) < 7)
            {
                bRecognizeWakgood = true;
                StopCoroutine(idle);
                StartCoroutine(attack = Attack());
                break;
            }
            yield return ws01;
        }
    }

    private IEnumerator Idle()
    { 
        Vector3 spawnPos = transform.position;

        while (true)
        {
            Animator.SetBool(ismoving, true);
            Vector2 targetPos = spawnPos + new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 2f), 0);
            Vector2 originPos = transform.position;
            SpriteRenderer.flipX = targetPos.x <= originPos.x;
            for (int i = 0; i < 50; i++)
            {
                Rigidbody2D.position += (targetPos - originPos) * 0.02f;
                yield return new WaitForSeconds(0.02f);
            }
            Animator.SetBool(ismoving, false);
            yield return new WaitForSeconds(2f);
        }
    }

    private IEnumerator Ahya()
    {
        Animator.SetBool(ismoving, false);
        yield return new WaitForSeconds(1f);
        if (bRecognizeWakgood)
            StartCoroutine(attack = Attack());
        else
            StartCoroutine(idle = Idle());
    }

    private IEnumerator Attack()
    {
        Animator.SetBool(ismoving, true);
        while (true)
        {
            SpriteRenderer.flipX = transform.position.x > Wakgood.Instance.transform.position.x;

            Vector2 moveDirection = (Wakgood.Instance.transform.position - transform.position).normalized;
            Rigidbody2D.velocity = moveDirection * MoveSpeed;
            yield return null;
        }
    }

    protected override void _ReceiveHit()
    {
        base._ReceiveHit();

        SpriteRenderer.flipX = transform.position.x > Wakgood.Instance.transform.position.x;

        StopCoroutine(idle);
        StopCoroutine(attack);
        StopCoroutine(ahya);
        StopCoroutine(checkWakgood);
        ahya = Ahya();
        StartCoroutine(ahya);       
    }

    protected override IEnumerator Collapse()
    {
        bodyCollider.enabled = false;
        StopAllCoroutines();
        return base.Collapse();
    }
}
