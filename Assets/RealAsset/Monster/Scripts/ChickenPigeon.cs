using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChickenPigeon : NormalMonster
{
    private enum State { Idle, TenceUp, Ahya, Attack }
    private State currentState = State.Idle;
    private bool stateChange = false;
    private bool bRecognizeTraveller = false;
    [SerializeField] private GameObject warningLine;

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
        // Debug.Log("Start Idle");
        float duration = Random.Range(0.5f, 2.5f + 0.1f);

        Vector3 moveDirection = Vector3.zero;
        bool bIsMoving = Random.Range(0, 1 + 1) == 0 ? true : false;

        animator.SetBool("IsMoving", bIsMoving);
        if (bIsMoving)
        {
            int r = Random.Range(0, 4);
            moveDirection = r == 0 ? Vector2.up : r == 1 ? Vector2.down : r == 2 ? Vector2.left : Vector2.right;
            spriteRenderer.flipX = r == 2 ? true : r == 3 ? false : spriteRenderer.flipX;
        }

        while (stateChange == false)
        {
            if (Vector2.Distance(transform.position, Wakgood.Instance.transform.position) < 10)
            {
                bRecognizeTraveller = true;
                SetState(State.TenceUp);
            }

            duration -= Time.deltaTime;
            if (duration <= 0)
            {
                SetState(State.Idle);
            }

            rigidbody2D.velocity = moveDirection * moveSpeed;
            yield return null;
        }
    }

    private IEnumerator TenceUp()
    {
        // Debug.Log("Start TenceUp");
        float duration = 6f;

        while (stateChange == false)
        {
            duration -= Time.deltaTime;
            if (duration <= 0)
            {
                SetState(State.Attack);
            }

            yield return null;
        }
    }

    private IEnumerator Ahya()
    {
        //Debug.Log("Start Ahya");
        float duration = 0.3f;

        while (stateChange == false)
        {
            duration -= Time.deltaTime;
            if (duration <= 0)
            {
                SetState(bRecognizeTraveller ? State.TenceUp : State.Idle);
            }
            yield return null;
        }
    }

    private IEnumerator Attack()
    {
        //Debug.Log("Start Attack");
        float duration = 0.6f;
        float delay = 1f;

        warningLine.SetActive(true);
        Vector2 rushDirection = (Wakgood.Instance.transform.position - transform.position).normalized;
        while (stateChange == false)
        {
            delay -= Time.deltaTime;
            if (delay <= 0)
            {
                duration -= Time.deltaTime;
                if (duration <= 0)
                {
                    SetState(State.TenceUp);
                }

                rigidbody2D.velocity = rushDirection * moveSpeed * 10;
            }

            yield return null;
        }
    }

    public override void ReceiveHit(int damage)
    {
        base.ReceiveHit(damage);

        if (transform.position.x > Wakgood.Instance.transform.position.x)
            spriteRenderer.flipX = true;
        else
            spriteRenderer.flipX = false;

        SetState(State.Ahya);
    }
}
