using System.Collections;
using UnityEngine;

public class Slime1 : Monster
{
    float ahyaDuration = 0;

    protected override void OnEnable()
    {
        base.OnEnable();
        StartCoroutine(Brain());
    }

    private IEnumerator Brain()
    {
        WaitForSeconds wait = new WaitForSeconds(0.1f);
        float stateDuration = 0;
        
        while (true)
        {
            if (ahyaDuration > 0)
            {
                ahyaDuration -= 0.1f;
            }
            else
            {
                if (stateDuration <= 0)
                {
                    if (Random.Range(0, 2) == 0)
                    {
                        animator.SetBool("IsMoving", false);
                    }
                    else
                    {
                        int r = Random.Range(0, 4);
                        Vector3 direction = r == 0 ? Vector2.up : r == 1 ? Vector2.down : r == 2 ? Vector2.left : Vector2.right;
                        spriteRenderer.flipX = r == 2 ? true : r == 3 ? false : spriteRenderer.flipX;

                        rigidbody2D.velocity = direction * moveSpeed;
                        animator.SetBool("IsMoving", true);
                    }

                    stateDuration = Random.Range(1f, 3f);
                }

                stateDuration -= 0.1f;
            }
            
            yield return wait;
        }
    }

    public override void ReceiveDamage(int damage, TextType damageType = TextType.Normal)
    {
        base.ReceiveDamage(damage, damageType);
        animator.SetTrigger("Ahya");
        ahyaDuration = 0.3f;
    }

    protected override void OnCollapse()
    {

    }
}
