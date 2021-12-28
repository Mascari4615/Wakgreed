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

    private IEnumerator Move( )
    {
        Vector2 spawnPos = transform.position;
        for (float j = 0; j <= 1; j += 0.02f)
        {
            SpriteRenderer.flipX = (Wakgood.Instance.transform.position.x > transform.position.x) ? true : false;
            Rigidbody2D.transform.position = Vector3.Lerp(spawnPos, Wakgood.Instance.transform.position, j);
            yield return new WaitForSeconds(0.02f);
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
