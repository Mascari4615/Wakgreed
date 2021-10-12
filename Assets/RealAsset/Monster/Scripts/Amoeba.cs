using System.Collections;
using UnityEngine;

public class Amoeba : NormalMonster
{
    private IEnumerator idle;
    private IEnumerator attack;
    private IEnumerator ahya;
    private bool bRecognizeWakgood = false;
    [SerializeField] private Collider2D bodyCollider;

    protected override void OnEnable()
    {
        base.OnEnable();

        bodyCollider.enabled = true;
        idle = Idle();
        StartCoroutine(idle);
        StartCoroutine(CheckWakgood());
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
                attack = Attack();
                StartCoroutine(attack);
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
            animator.SetBool("ISMOVING", true);
            Vector2 targetPos = spawnPos + new Vector3(Random.Range(-2f, 2f), Random.Range(-2f, 2f), 0);
            Vector2 originPos = transform.position;
            spriteRenderer.flipX = (targetPos.x > originPos.x) ? false : true;
            for (int i = 0; i < 50; i++)
            {
                rigidbody2D.position += (targetPos - originPos) * 0.02f;
                yield return new WaitForSeconds(0.02f);
            }
            animator.SetBool("ISMOVING", false);
            yield return new WaitForSeconds(2f);
        }
    }

    private IEnumerator Ahya()
    {
        animator.SetBool("ISMOVING", false);
        yield return new WaitForSeconds(1f);
        if (bRecognizeWakgood)
        {
            attack = Attack();
            StartCoroutine(attack);
        }
        else
        {          
            idle = Idle();
            StartCoroutine(idle);
        }   
    }

    private IEnumerator Attack()
    {
        animator.SetBool("ISMOVING", true);
        while (true)
        {
            spriteRenderer.flipX = transform.position.x > Wakgood.Instance.transform.position.x ? true : false;

            Vector2 moveDirection = (Wakgood.Instance.transform.position - transform.position).normalized;
            rigidbody2D.velocity = moveDirection * moveSpeed;
            yield return null;
        }
    }

    public override void ReceiveDamage(int damage)
    {
        spriteRenderer.flipX = transform.position.x > Wakgood.Instance.transform.position.x ? true : false;

        StopAllCoroutines();
        ahya = Ahya();
        StartCoroutine(ahya);

        base.ReceiveDamage(damage);      
    }

    protected override IEnumerator Collapse()
    {
        bodyCollider.enabled = false;
        StopAllCoroutines();
        return base.Collapse();
    }
}
