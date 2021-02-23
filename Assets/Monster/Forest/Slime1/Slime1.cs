using System.Collections;
using UnityEngine;

public class Slime1 : Monster
{
    private enum State {Idle, Ahya, RandomMove, Attack}
    private State currentState = State.Idle;
    private bool stateChange = false;

    protected override void OnEnable()
    {
        base.OnEnable();
        
        StartCoroutine(Brain());
    }

    private IEnumerator Brain()
    {
        while (true)
        {
            stateChange = false;
            yield return StartCoroutine(currentState.ToString());
        }
    }

    private void SetState(State newState)
    {
        currentState = newState;
        stateChange = true;
    }

    private IEnumerator Idle()
    {
        Debug.Log("Start Idle");
        float duration = Random.Range(0.5f, 2.5f + 0.1f);
        animator.SetBool("IsMoving", false);

        while (stateChange == false)
        {
            duration -= Time.deltaTime;
            if (duration <= 0)
            {
                State randomState = (Random.Range(0, 1 + 1) == 0) ? State.Idle : State.RandomMove;
                SetState(randomState);
            }
            yield return null;
        }
    }

    private IEnumerator Ahya()
    {
        Debug.Log("Start Ahya");
        float duration = 0.3f;

        while (stateChange == false)
        {
            duration -= Time.deltaTime;
            if (duration <= 0)
            {
                State randomState = (Random.Range(0, 1 + 1) == 0) ? State.Idle : State.RandomMove;
                SetState(randomState);
            }
            yield return null;
        }
    }

    private IEnumerator RandomMove()
    {
        Debug.Log("Start RandomMove");
        float duration = Random.Range(0.5f, 2.5f + 0.1f);
        animator.SetBool("IsMoving", true);
        int r = Random.Range(0, 4);
        Vector3 moveDirection = r == 0 ? Vector2.up : r == 1 ? Vector2.down : r == 2 ? Vector2.left : Vector2.right;
        spriteRenderer.flipX = r == 2 ? true : r == 3 ? false : spriteRenderer.flipX;

        while (stateChange == false)
        {
            rigidbody2D.velocity = moveDirection * moveSpeed;
            
            duration -= Time.deltaTime;
            if (duration <= 0)
            {
                State randomState = (Random.Range(0, 1 + 1) == 0) ? State.Idle : State.RandomMove;
                SetState(randomState);
            }
            yield return null;
        }
    }
    
    public override void ReceiveDamage(int damage, TextType damageType = TextType.Normal)
    {
        base.ReceiveDamage(damage, damageType);

        if (transform.position.x > TravellerController.Instance.transform.position.x)
            spriteRenderer.flipX = true;
        else
            spriteRenderer.flipX = false;

        SetState(State.Ahya);
    }

    protected override void OnCollapse()
    {

    }
}
