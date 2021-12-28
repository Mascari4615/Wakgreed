using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuriswordPanchi : NormalMonster
{
    Vector3 spawnedPos = Vector3.zero;
    float moveLimit = 3f;
    [SerializeField] LineRenderer lineRenderer;

    protected override void OnEnable()
    {
        base.OnEnable();
        spawnedPos = transform.position;
        lineRenderer.material.SetColor("_Color", new Color(1f, 1f, 1f, 0.3f));

        StartCoroutine(Attack());
    }

    private IEnumerator Attack()
    {
        while (true)
        {
            while (Vector2.Distance(Wakgood.Instance.transform.position, transform.position) > 5f)
            {
                Rigidbody2D.velocity = (Wakgood.Instance.transform.position - transform.position).normalized * MoveSpeed;
                yield return null;
            }

            Rigidbody2D.velocity = Vector2.zero;
            yield return StartCoroutine(Skill0());
        }
    }

    private IEnumerator Skill0()
    {
        // Animator.SetBool("SKILL1", true);

        Vector3 originPos = transform.position;
        Vector3 targetPos = Vector3.ClampMagnitude(spawnedPos + (Vector3)Random.insideUnitCircle * 2f, moveLimit);

        // Animator.SetTrigger("SKILL1CHARGE");

        SpriteRenderer.flipX = targetPos.x > Wakgood.Instance.transform.position.x;

        for (float j = 0; j <= 1; j += Time.deltaTime * 7)
        {
            Rigidbody2D.transform.position = Vector3.Lerp(originPos, targetPos, j);
            yield return null;
        }

        Vector3 attackDirection = (Wakgood.Instance.transform.position - transform.position).normalized;

        lineRenderer.SetPosition(0, transform.position + (Vector3)Vector2.up);
        lineRenderer.SetPosition(1, transform.position + (Vector3)Vector2.up + attackDirection * 100);
        lineRenderer.gameObject.SetActive(true);

        yield return StartCoroutine(Casting(0.7f));

        // Animator.SetTrigger("SKILL1GO");

        var a = ObjectManager.Instance.PopObject("Suri", transform.position);

        a.transform.position = transform.position + (Vector3)Vector2.up;
        a.GetComponent<BulletMove>().SetDirection(attackDirection);
        lineRenderer.gameObject.SetActive(false);

        // Animator.SetBool("SKILL1", false);
        yield return new WaitForSeconds(5f);
    }
}
