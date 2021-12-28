using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : NormalMonster
{
    private Coroutine move;

    protected override void OnEnable()
    {
        base.OnEnable();

        move = StartCoroutine(Move());
    }

    private IEnumerator Move()
    {
        Vector2 spawnPos = transform.position;
        float t = 0;

        while (true)
        {
            yield return null;
            t += Time.deltaTime;
            transform.position = Vector3.Lerp(spawnPos, transform.position, t);
            SpriteRenderer.flipX = (Wakgood.Instance.transform.position.x > transform.position.x) ? true : false;
        }
    }

    protected override void _ReceiveHit()
    {
        base._ReceiveHit();
        transform.position = Wakgood.Instance.transform.position + (Vector3) Random.insideUnitCircle * 20f;
        if (move!=null) StartCoroutine(Move());
        move = StartCoroutine(Move());
    }
}
