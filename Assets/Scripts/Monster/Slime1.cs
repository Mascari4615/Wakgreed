using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slime1 : Monster
{
    Vector3 direction = Vector3.zero;

    protected override void OnEnable()
    {
        base.OnEnable();
        StartCoroutine("ChangeDirection");
    }

    protected override void Update()
    {
        base.Update();
        monsterRigidbody2D.velocity = direction * moveSpeed;
    }

    protected override void InsertQueue()
    {
        base.InsertQueue();
        ObjectManager.Instance.InsertQueue(PoolType.Slime1, gameObject);
    }

    IEnumerator ChangeDirection()
    {
        int rand = Random.Range(0,4);

        if (rand == 0)
            direction = Vector2.up;
        else if (rand == 1)
            direction = Vector2.down;
        else if (rand == 2)
        {
            direction = Vector2.left;
            spriteRenderer.flipX = true;
        }
        else if (rand == 3)
        {
            direction = Vector2.right;
            spriteRenderer.flipX = false;
        }

        yield return new WaitForSeconds(2f);
        StartCoroutine("ChangeDirection");
    }
}
