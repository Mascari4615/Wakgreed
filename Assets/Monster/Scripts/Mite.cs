using System.Collections;
using UnityEngine;

public class Mite : NormalMonster
{
    protected override void OnEnable()
    {
        base.OnEnable();
        Animator.SetBool("MOVING", false);
        StartCoroutine(Attack());
    }

    private IEnumerator Attack()
    {
        while (true)
        {
            if (Vector2.Distance(transform.position, Wakgood.Instance.transform.position) > 2)
            {
                SpriteRenderer.flipX = Rigidbody2D.velocity.x > 0;
                Animator.SetBool("MOVING", true);
                Rigidbody2D.velocity = ((Vector2)Wakgood.Instance.transform.position - Rigidbody2D.position).normalized * MoveSpeed;
                yield return null;
            }
            else
            {
                Animator.SetBool("MOVING", false);
                Rigidbody2D.velocity = Vector2.zero;
                yield return StartCoroutine(Casting(castingTime));
                ObjectManager.Instance.PopObject("MiteAttack", transform.position + Vector3.up * 0.8f + GetDirection() * 1.5f, GetRot());
                yield return new WaitForSeconds(1f);
            }
        }
    }
}