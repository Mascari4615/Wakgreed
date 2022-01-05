using System.Collections;
using UnityEngine;

public class SuriswordPanchi : NormalMonster
{
    [SerializeField] LineRenderer lineRenderer;
    private Coroutine moveCO;
    private Coroutine attackCO;

    protected override void OnEnable()
    {
        base.OnEnable();
        lineRenderer.material.SetColor("_Color", new Color(1f, 1f, 1f, 0.6f));
        Animator.SetBool("MOVING", false);
        Animator.SetTrigger("AWAKE");
        StartCoroutine(Move());
    }

    private IEnumerator Move()
    {
        while (true)
        {
            Animator.SetBool("MOVING", true);
            while (Vector2.Distance(Wakgood.Instance.transform.position, transform.position) > 5f)
            {
                SpriteRenderer.flipX = transform.position.x < Wakgood.Instance.transform.position.x;
                Rigidbody2D.velocity = (Wakgood.Instance.transform.position - transform.position).normalized * MoveSpeed;
                yield return null;
            }
            Animator.SetBool("MOVING", false);

            Rigidbody2D.velocity = Vector2.zero;
            yield return StartCoroutine(Attack());
        }
    }

    private IEnumerator Attack()
    {
        Vector3 deirection = Random.insideUnitCircle;

        SpriteRenderer.flipX = (transform.position + (Vector3)Random.insideUnitCircle * 2f).x < Wakgood.Instance.transform.position.x;

        for (float j = 0; j <= 1; j += Time.deltaTime * 7)
        {
            Rigidbody2D.velocity = deirection * MoveSpeed;
            yield return null;
        }

        Vector3 attackDirection = (Wakgood.Instance.transform.position - transform.position).normalized;
        Rigidbody2D.velocity = Vector3.zero;
        lineRenderer.SetPosition(0, transform.position + (Vector3)Vector2.up);
        lineRenderer.SetPosition(1, transform.position + (Vector3)Vector2.up + attackDirection * 100);
        lineRenderer.gameObject.SetActive(true);

        Animator.SetTrigger("READY");

        yield return StartCoroutine(Casting(0.7f));

        Animator.SetTrigger("GO");

        var a = ObjectManager.Instance.PopObject("Suri", transform.position);

        a.transform.position = transform.position + (Vector3)Vector2.up;
        a.GetComponent<BulletMove>().SetDirection(attackDirection);
        lineRenderer.gameObject.SetActive(false);

        yield return new WaitForSeconds(5f);
    }

    protected override void OnDisable()
    {
        if (attackCO != null) StopCoroutine(attackCO);
        if (moveCO != null) StopCoroutine(moveCO);
        base.OnDisable();
    }
}
