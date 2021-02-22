using System.Collections;
using UnityEngine;

public class Slime1 : Monster
{
    int ahyaDuration = 0;

    protected override void OnEnable()
    {
        base.OnEnable();
        StartCoroutine(Brain());
    }

    private IEnumerator Brain()
    {
        WaitForSeconds wait = new WaitForSeconds(0.1f);
        float stateDuration = 0;
        bool isMoving = false;
        Vector3 moveDirection = Vector3.zero;
        bool lastFlipX = false;
        
        while (true)
        {
            if (ahyaDuration > 0)
            {
                Debug.Log(ahyaDuration);
                ahyaDuration -= 1;
            }
            else
            {
                if (stateDuration <= 0)
                {
                    if (Random.Range(0, 5) == 0)
                    {
                        animator.SetBool("IsMoving", false);
                        isMoving = false;
                    }
                    else
                    {
                        int r = Random.Range(0, 4);
                        moveDirection = r == 0 ? Vector2.up : r == 1 ? Vector2.down : r == 2 ? Vector2.left : Vector2.right;
                        spriteRenderer.flipX = r == 2 ? true : r == 3 ? false : spriteRenderer.flipX;
                        lastFlipX = spriteRenderer.flipX;

                        animator.SetBool("IsMoving", true);
                        isMoving = true;
                    }

                    stateDuration = Random.Range(1f, 3f);
                }

                if (isMoving)
                {
                    rigidbody2D.velocity = moveDirection * moveSpeed;
                    spriteRenderer.flipX = lastFlipX;
                }

                stateDuration -= 0.1f;
            }
            
            yield return wait;
        }
    }
    
    public override void ReceiveDamage(int damage, TextType damageType = TextType.Normal)
    {
        base.ReceiveDamage(damage, damageType);

        if (transform.position.x > TravellerController.Instance.transform.position.x)
            spriteRenderer.flipX = true;
        else
            spriteRenderer.flipX = false;

        ahyaDuration = 4;
    }

    protected override void OnCollapse()
    {

    }
}
