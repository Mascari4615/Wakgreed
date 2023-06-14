using System.Collections;
using UnityEngine;

public class Ghost : NormalMonster
{
    private Coroutine move;

    protected override void OnEnable()
    {
        base.OnEnable();
        move = StartCoroutine(Move());
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            transform.position = Wakgood.Instance.transform.position + ((Vector3)Random.insideUnitCircle * 20f);
            if (move != null)
            {
                StartCoroutine(Move());
            }

            move = StartCoroutine(Move());
        }
    }

    private IEnumerator Move()
    {
        while (true)
        {
            Rigidbody2D.velocity = (Wakgood.Instance.transform.position - transform.position).normalized * MoveSpeed;
            yield return new WaitForSeconds(0.1f);
            SpriteRenderer.flipX = Wakgood.Instance.transform.position.x > transform.position.x;
        }
    }

    protected override void _ReceiveHit()
    {
        base._ReceiveHit();
        transform.position = Wakgood.Instance.transform.position + ((Vector3)Random.insideUnitCircle * 20f);
        if (move != null)
        {
            StartCoroutine(Move());
        }

        move = StartCoroutine(Move());
    }
}