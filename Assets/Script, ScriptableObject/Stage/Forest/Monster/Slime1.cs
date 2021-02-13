using System.Collections;
using UnityEngine;

public class Slime1 : Monster
{
    protected override void OnEnable()
    {
        base.OnEnable();
        StartCoroutine(ChangeDirection());
    }

    private IEnumerator ChangeDirection()
    {
        int r = Random.Range(0, 4);
        Vector3 direction = r == 0 ? Vector2.up : r == 1 ? Vector2.down : r == 2 ? Vector2.left : Vector2.right;
        spriteRenderer.flipX = r == 2 ? true : r == 3 ? false : spriteRenderer.flipX;

        rigidbody2D.velocity = direction * moveSpeed;

        yield return new WaitForSeconds(2f);
        StartCoroutine("ChangeDirection");
    }
}
